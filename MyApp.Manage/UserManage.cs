using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DayuCloud.Model.Common;
using MyApp.ServiceModel.Common;
using MyApp.ServiceModel.Models;
using MyApp.ServiceModel.Org;
using MyApp.ServiceModel.User;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.OrmLite;

namespace MyApp.Manage
{
    public class UserManage : ManageBase, IUserManage
    {
        public async Task<PagedResult<UserResDto>> GetUsers(UserQueryDto queryDto)
        {
            using (var db = DbFactory.Open())
            {
                var builder = db.From<UserEntity>();
                if (queryDto.OrganizationId.HasValue)
                {
                    builder.Where(x => x.OrganizationId == queryDto.OrganizationId);
                }

                if (!string.IsNullOrEmpty(queryDto.SearchName))
                {
                    builder.Where(x => x.UserName.Contains(queryDto.SearchName) ||
                                       x.DisplayName.Contains(queryDto.SearchName));
                }

                if (queryDto.Valid.HasValue)
                {
                    if (queryDto.Valid == true)
                    {
                        builder.Where(x => x.LockedDate == null);
                    }
                    else
                    {
                        builder.Where(x => x.LockedDate != null);
                    }
                }

                if (!string.IsNullOrEmpty(queryDto.Role))
                {
                    builder.Where(x => x.Role == queryDto.Role);
                }

                builder.OrderBy(x => x.Id);
                var count = await db.CountAsync(builder);

                builder.Limit(queryDto.Skip, queryDto.PageSize);
                var users = await db.SelectAsync(builder);

                // 准备数据
                var userDtos = await ConvertToUserResponseDto(db, users);

                return new PagedResult<UserResDto>
                {
                    Total = count,
                    Results = userDtos,
                    PageIndex = queryDto.PageIndex,
                    PageSize = queryDto.PageSize
                };
            }
        }

        public async Task<UserResDto> GetUserById(long userId)
        {
            using (var db = DbFactory.Open())
            {
                var user = await db.SingleByIdAsync<UserEntity>(userId);
                if (user == null)
                {
                    return await Task.FromResult<UserResDto>(null);
                }

                var userDtos = await ConvertToUserResponseDto(db, new List<UserEntity> {user});
                return await Task.FromResult(userDtos.First());
            }
        }

        private static async Task<List<UserResDto>> ConvertToUserResponseDto(IDbConnection db,
            IReadOnlyCollection<UserEntity> users)
        {
            if (users == null) return null;
            if (users.Count == 0) return new List<UserResDto>();

            var resDtos = new List<UserResDto>(users.Count);

            var userIds = users.Select(x => x.Id).ToHashSet();
            // 组织列表
            var orgIds = users.Select(x => x.OrganizationId).ToHashSet();
            if (orgIds.Count == 0) return null;
            var orgs = await db.SelectAsync<OrganizationEntity>(x => Sql.In(x.Id, orgIds));

            foreach (var user in users)
            {
                var userDto = user.ConvertTo<UserResDto>();

                userDto.Org = orgs.FirstOrDefault(x => x.Id == user.OrganizationId);
                userDto.UseStatus = user.LockedDate.HasValue ? UseStatus.Invalid : UseStatus.Valid;

                resDtos.Add(userDto);
            }

            return resDtos;
        }

        /// <summary>
        /// 密码验证规则. 必须大于6位.
        /// </summary>
        private static async Task<bool> ValidatePassword(string password)
        {
            var result = !string.IsNullOrEmpty(password);

            if (result && password.Length < 6)
            {
                result = false;
            }

            return await Task.FromResult(result);
        }


        public async Task<long> SaveUser(UserSaveDto userSaveDto)
        {
            var isNew = userSaveDto.Id <= 0;
            // 处理验证
            if (userSaveDto.Role != RoleConstants.Admin &&
                userSaveDto.Role != RoleConstants.Operator &&
                userSaveDto.Role != RoleConstants.Watcher)
            {
                throw new UserFriendlyException("无效的角色");
            }

            if (string.IsNullOrEmpty(userSaveDto.DisplayName))
            {
                throw new UserFriendlyException("用户姓名不能为空.");
            }

            if (isNew)
            {
                var pwdValid = await ValidatePassword(userSaveDto.Password);
                if (!pwdValid)
                {
                    throw new UserFriendlyException("密码必须不小于6位.");
                }
            }

            using (var db = DbFactory.Open())
            {
                // 用户名不能变更
                if (!isNew)
                {
                    var userName = await db.ScalarAsync<string>(db.From<UserEntity>().Where(x => x.Id == userSaveDto.Id)
                        .Select(x => x.UserName));
                    if (userSaveDto.UserName != userName)
                    {
                        throw new UserFriendlyException("修改时用户名不能变更.");
                    }
                }

                // 用户名不能重复.
                var userExists = isNew
                    ? await db.ExistsAsync<UserEntity>(x => x.UserName == userSaveDto.UserName)
                    : await db.ExistsAsync<UserEntity>(
                        x => x.Id != userSaveDto.Id && x.UserName == userSaveDto.UserName);

                if (userExists)
                {
                    throw new UserFriendlyException($"用户名:{userSaveDto.UserName},已存在.请换个用户名.");
                }

                //组织必须存在
                var orgExists = await db.ExistsAsync<OrganizationEntity>(x => x.Id == userSaveDto.OrganizationId);
                if (!orgExists)
                {
                    throw new UserFriendlyException("组织不存在.");
                }

                var authRepo =
                    (OrmLiteAuthRepository<UserEntity, UserAuthDetails>) HostContext.AppHost.GetAuthRepository();

                using (var trans = db.OpenTransaction())
                {
                    if (isNew)
                    {
                        var user = userSaveDto.ConvertTo<UserEntity>();
                        // 设置邮箱和锁定状态
                        user.Email = $"{userSaveDto.UserName}@dayu.com";
                        if (userSaveDto.UseStatus == UseStatus.Invalid)
                        {
                            user.LockedDate = DateTime.UtcNow;
                        }

                        var newUser = authRepo.CreateUserAuth(user, userSaveDto.Password);

                        userSaveDto.Id = newUser.Id;
                    }
                    else
                    {
                        var oldUser = await db.SingleByIdAsync<UserEntity>(userSaveDto.Id);
                        var newUser = oldUser.CreateCopy();
                        newUser.DisplayName = userSaveDto.DisplayName;
                        newUser.OrganizationId = userSaveDto.OrganizationId;
                        newUser.Role = userSaveDto.Role;

                        if (userSaveDto.UseStatus == UseStatus.Invalid)
                        {
                            newUser.LockedDate = DateTime.UtcNow;
                        }

                        authRepo.UpdateUserAuth(oldUser, newUser);
                    }

                    trans.Commit();

                    return userSaveDto.Id;
                }
            }
        }

        public async Task ChangePassword(long userId, string oldPassword, string newPassword)
        {
            using (var db = DbFactory.Open())
            {
                var user = await db.SingleByIdAsync<UserEntity>(userId);
                if (user == null)
                {
                    throw new UserFriendlyException("用户不存在.");
                }

                var pwdValid = await ValidatePassword(newPassword);
                if (!pwdValid)
                {
                    throw new UserFriendlyException("密码不小于6位.");
                }

                var pwdCheck = user.VerifyPassword(oldPassword, out var _);
                if (!pwdCheck)
                {
                    throw new UserFriendlyException("旧密码不匹配");
                }

                var authRepo =
                    (OrmLiteAuthRepository<UserEntity, UserAuthDetails>) HostContext.AppHost.GetAuthRepository();
                user.InvalidLoginAttempts = 0;

                authRepo.UpdateUserAuth(user, user, newPassword);
            }
        }

        public async Task DeleteUser(long id)
        {
            using (var db = DbFactory.Open())
            {
                var exists = await db.ExistsAsync<UserEntity>(x => x.Id == id);
                if (!exists)
                {
                    throw new UserFriendlyException("用户不存在.");
                }

                //TODO: 判断哪些情况不能删除用户.

                await db.DeleteByIdAsync<UserEntity>(id);
            }
        }
    }
}