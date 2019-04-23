using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MyApp.ServiceModel.District;
using MyApp.ServiceModel.Permission;
using MyApp.ServiceModel.Role;
using MyApp.ServiceModel.User;
using MyApp.ServiceModel.Common;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.FluentValidation;
using ServiceStack.FluentValidation.Results;
using ServiceStack.OrmLite;

namespace MyApp.Manage
{
    public class LocalAccountManage<T> : ManageBase, IAccountManage where T : User
    {
        public IValidator<UserSaveDto> UserValidator { get; set; }
        public IValidator<RoleSaveDto> RoleValidator { get; set; }
        public IValidator<RoleGroupSaveDto> RoleGroupValidator { get; set; }


        /// <summary>
        /// 添加其它的用户过滤条件.重写该方法以添加其它过滤条件。
        /// </summary>
        /// <param name="queryDto">用户查询对象，可以是继承自 UserQueryDto 的子类实例。</param>
        /// <param name="builder">基于 ServiceStack 的 SqlExpress，可以添加其它过滤条件。</param>
        /// <returns></returns>
        protected virtual async Task FilterUserAsync(UserQueryDto queryDto, SqlExpression<T> builder)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 基于已读取的用户返回对象，添加额外的返回信息
        /// </summary>
        /// <param name="db">数据库连接</param>
        /// <param name="userResDtos">原始的用户返回对象</param>
        /// <returns>可以返回继承用户返回对象子类的列表</returns>
        protected virtual async Task<List<UserResDto>> SetUserResDtosAsync(IDbConnection db,
            List<UserResDto> userResDtos)
        {
            return await Task.FromResult(userResDtos);
        }

        /// <summary>
        /// 基于已读取的用户对象，添加额外的返回信息
        /// </summary>
        /// <param name="db">数据库连接</param>
        /// <param name="userResDto">单个用户的返回对象</param>
        /// <returns>可返回额外的用户信息</returns>
        protected virtual async Task<UserResDto> SetUserResDtoAsync(IDbConnection db, UserResDto userResDto)
        {
            return await Task.FromResult(userResDto);
        }

        /// <summary>
        /// 保存其它的用户信息
        /// </summary>
        /// <param name="db">数据库连接</param>
        /// <param name="userSaveDto">用户保存对象</param>
        /// <typeparam name="T">User 或它的子类</typeparam>
        /// <returns>Task</returns>
        protected virtual async Task SaveOtherUserInfoAsync(IDbConnection db, UserSaveDto userSaveDto)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 额外的用户验证方法
        /// </summary>
        /// <param name="userSaveDto">用户保存对象</param>
        /// <returns>验证结果</returns>
        protected virtual async Task<ValidationResult> ValidateUserAsync(UserSaveDto userSaveDto)
        {
            return await UserValidator.ValidateAsync(userSaveDto);
        }

        /// <summary>
        /// 是否可以删除用户
        /// </summary>
        /// <param name="db">数据库连接</param>
        /// <param name="user">待删除的用户</param>
        /// <typeparam name="T">用户类型</typeparam>
        /// <returns>是否</returns>
        protected virtual async Task<bool> CanDeleteUserAsync(IDbConnection db, T user)
        {
            return await Task.FromResult(true);
        }

        public virtual async Task<PagedResult<UserResDto>> GetUsers(UserQueryDto queryDto)
        {
            using (var db = GetConnection())
            {
                var builder = db.From<T>();

                if (queryDto.ValidStatus == ValidStatus.Invalid)
                {
                    builder.Where(x => x.LockedDate != null);
                }
                else if (queryDto.ValidStatus == ValidStatus.Valid)
                {
                    builder.Where(x => x.LockedDate == null);
                }

                if (!string.IsNullOrEmpty(queryDto.SearchName))
                {
                    builder.Where<User>(x => x.UserName.Contains(queryDto.SearchName) ||
                                             x.DisplayName.Contains(queryDto.SearchName));
                }

                builder.OrderBy<User>(x => x.Id);

                // 可以添加其它的过滤条件
                await FilterUserAsync(queryDto, builder);

                var count = await db.CountAsync(builder);

                builder.Limit(queryDto.Skip, queryDto.PageSize);
                var users = await db.SelectAsync(builder);
                var userResDtos = new List<UserResDto>();
                users.ForEach(user =>
                {
                    var resDto = user.ConvertTo<UserResDto>();
                    resDto.ValidStatus = user.LockedDate.HasValue ? ValidStatus.Invalid : ValidStatus.Valid;
                    userResDtos.Add(resDto);
                });

                // 可以添加更多的用户信息
                userResDtos = await SetUserResDtosAsync(db, userResDtos);

                return new PagedResult<UserResDto>
                {
                    Total = count,
                    Results = userResDtos,
                    PageIndex = queryDto.PageIndex,
                    PageSize = queryDto.PageSize
                };
            }
        }

        public virtual async Task<UserResDto> GetUserById(int userId)
        {
            using (var db = GetConnection())
            {
                var user = await db.SingleByIdAsync<User>(userId);
                if (user == null)
                {
                    throw new UserFriendlyException("用户不存在。");
                }

                var userResDto = user.ConvertTo<UserResDto>();
                userResDto.ValidStatus = user.LockedDate.HasValue ? ValidStatus.Invalid : ValidStatus.Valid;

                userResDto = await SetUserResDtoAsync(db, userResDto);

                return await Task.FromResult(userResDto);
            }
        }

        /// <summary>
        /// 密码验证规则. 必须大于6位.
        /// </summary>
        protected virtual async Task<bool> ValidatePassword(string password)
        {
            var result = !string.IsNullOrEmpty(password);

            if (result && password.Length < 2)
            {
                result = false;
            }

            return await Task.FromResult(result);
        }


        public virtual async Task<int> SaveUser(UserSaveDto userSaveDto)
        {
            var validationResult = await ValidateUserAsync(userSaveDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var isNew = !userSaveDto.Id.HasValue || userSaveDto.Id <= 0;

            if (isNew)
            {
                var pwdValid = await ValidatePassword(userSaveDto.Password);
                if (!pwdValid)
                {
                    throw new UserFriendlyException("密码必须不小于2位.");
                }
            }

            using (var db = GetConnection())
            {
                // 系统只有一张全局的用户表，各个应用可以有自己的用户，这些应用中的用户只能是全局用户表中的引用。
                // 应用新增用户时，如果发现该用户名已经在全局用户表中存在，则直接添加引用。并更新其它属性字段。
                if (!isNew)
                {
                    // 用户名不能变更
                    var userName = await db.ScalarAsync<string>(db.From<T>().Where(x => x.Id == userSaveDto.Id)
                        .Select(x => x.UserName));
                    if (userSaveDto.UserName != userName)
                    {
                        throw new UserFriendlyException("修改时用户名不能变更.");
                    }
                }

                // 用户名不能重复
                var userExists = isNew
                    ? await db.ExistsAsync<T>(x => x.UserName == userSaveDto.UserName)
                    : await db.ExistsAsync<T>(x => x.UserName == userSaveDto.UserName && x.Id != userSaveDto.Id);
                if (userExists)
                {
                    throw new UserFriendlyException($"用户名: {userSaveDto.UserName} 已存在，保存失败。");
                }

                var authRepo =
                    (OrmLiteAuthRepository<T, UserAuthDetails>) HostContext.AppHost.GetAuthRepository();

                var dbUser = await db.SingleAsync<T>(x => x.UserName == userSaveDto.UserName);

                using (var trans = db.OpenTransaction())
                {
                    if (isNew)
                    {
                        var user = userSaveDto.ConvertTo<User>();
                        // 设置邮箱和锁定状态
                        user.Email = $"{userSaveDto.UserName}@dayu.com";
                        dbUser = (T) authRepo.CreateUserAuth(user, userSaveDto.Password);
                        userSaveDto.Id = dbUser.Id;
                    }
                    else
                    {
                        if (dbUser == null)
                        {
                            throw new UserFriendlyException("待更新的用户不存在。");
                        }

                        // 更新部分字段
                        dbUser.ModifiedDate = DateTime.Now;
                        dbUser.DisplayName = userSaveDto.DisplayName;
                        if (userSaveDto.ValidStatus == ValidStatus.Invalid)
                        {
                            dbUser.LockedDate = DateTime.Now;
                        }
                        else
                        {
                            dbUser.LockedDate = null;
                            dbUser.InvalidLoginAttempts = 0;
                        }

                        await db.UpdateOnlyAsync(dbUser, onlyFields: x => new
                            {
                                x.ModifiedDate,
                                x.DisplayName,
                                x.LockedDate,
                                x.InvalidLoginAttempts
                            },
                            @where: x => x.Id == dbUser.Id);
                    }

                    trans.Commit();

                    return userSaveDto.Id ?? 0;
                }
            }
        }


        public virtual async Task<int> ChangePassword(UserChangePasswordSaveDto request)
        {
            using (var db = GetConnection())
            {
                var user = await db.SingleByIdAsync<T>(request.UserId);
                if (user == null)
                {
                    throw new UserFriendlyException("用户不存在.");
                }

                var pwdValid = await ValidatePassword(request.NewPassword);
                if (!pwdValid)
                {
                    throw new UserFriendlyException("密码不小于2位.");
                }

                var passed = user.VerifyPassword(request.OldPassword, out var needsRehash);
                if (!passed)
                {
                    throw new UserFriendlyException("旧密码不正确。");
                }

                var authRepo = (OrmLiteAuthRepository<T, UserAuthDetails>) HostContext.AppHost.GetAuthRepository();

                authRepo.UpdateUserAuth(user, user, request.NewPassword);

                return request.UserId;
            }
        }

        public async Task<int> ResetPassword(UserResetPasswordSaveDto request)
        {
            using (var db = GetConnection())
            {
                var user = await db.SingleByIdAsync<T>(request.UserId);
                if (user == null)
                {
                    throw new UserFriendlyException("用户不存在.");
                }

                var pwdValid = await ValidatePassword(request.Password);
                if (!pwdValid)
                {
                    throw new UserFriendlyException("密码不小于2位.");
                }

                var authRepo = (OrmLiteAuthRepository<T, UserAuthDetails>) HostContext.AppHost.GetAuthRepository();
                authRepo.UpdateUserAuth(user, user, request.Password);

                return request.UserId;
            }
        }

        public virtual async Task<int> DeleteUser(int id, string appKey)
        {
            using (var db = GetConnection())
            {
                var user = await db.SingleByIdAsync<T>(id);
                if (user == null)
                {
                    throw new UserFriendlyException("用户不存在.");
                }

                var canDelete = await CanDeleteUserAsync(db, user);
                if (!canDelete)
                {
                    return 0;
                }

                //TODO: 判断其它情况不能删除用户.

                await db.DeleteByIdAsync<T>(id);

                return id;
            }
        }

        private async Task GenRoleGroupsAsync(
            IDbConnection db,
            Node<RoleGroupResDto> node,
            IList<RoleGroupResDto> needRoleGroups,
            long parentId)
        {
            var groups = await db.SelectAsync<RoleGroup>(x => x.ParentId == parentId);
            foreach (var roleGroup in groups)
            {
                var resDto = roleGroup.ConvertTo<RoleGroupResDto>();
                if (!roleGroup.HasSubGroup)
                {
                    needRoleGroups.Add(resDto);
                }

                var lastNode = node.AddChild(resDto);
                await GenRoleGroupsAsync(db, lastNode, needRoleGroups, roleGroup.Id);
            }
        }

        public async Task<Node<RoleGroupResDto>> GetRoleGroups(RoleGroupQueryDto query)
        {
            var root = new Node<RoleGroupResDto>(new RoleGroupResDto
            {
                Id = 0L,
                ParentId = 0L,
                Name = "全部"
            });

            var needRoleGroups = new List<RoleGroupResDto>();
            using (var db = GetConnection())
            {
                await GenRoleGroupsAsync(db, root, needRoleGroups, 0L);

                if (!query.WithRoles || needRoleGroups.Count <= 0) return root;

                // 设置分组下的具体角色
                var gIds = needRoleGroups.Select(x => x.Id).ToList();
                var roles = await db.SelectAsync<Role>(x => Sql.In(x.RoleGroupId, gIds));
                if (roles.Count > 0)
                {
                    needRoleGroups.ForEach(g =>
                    {
                        g.Roles = roles.Where(x => x.RoleGroupId == g.Id)
                            .Select(x => new RoleSimpleResDto
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Desc = x.Desc
                            })
                            .ToList();
                    });
                }
            }

            return root;
        }

        public async Task<long> SaveRoleGroup(RoleGroupSaveDto request)
        {
            var validationResult = await RoleGroupValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            using (var db = GetConnection())
            {
                var isEdit = request.Id.HasValue && request.Id > 0L;

                var nameExists = isEdit
                    ? await db.ExistsAsync<RoleGroup>(x => x.ParentId == request.ParentId && x.Name == request.Name &&
                                                           x.Id != request.Id)
                    : await db.ExistsAsync<RoleGroup>(x => x.ParentId == request.ParentId && x.Name == request.Name);

                if (nameExists)
                {
                    throw new UserFriendlyException($"角色分组: {request.Name}，已存在，请更换名称。");
                }

                var parentGroup = await db.SingleByIdAsync<RoleGroup>(request.ParentId);
                if (request.ParentId > 0)
                {
                    if (parentGroup == null)
                    {
                        throw new UserFriendlyException("父分组不存在。");
                    }
                }

                if (isEdit)
                {
                    var dbGroup = await db.SingleByIdAsync<RoleGroup>(request.Id);
                    if (dbGroup == null)
                    {
                        throw new UserFriendlyException("角色分组不存在。");
                    }

                    if (dbGroup.ParentId != request.ParentId)
                    {
                        // 变更父分组时，如果父分组下包含授权项，则不能变更。因为分组下只能全部是子分组或授权项。
                        var hasPermission = await db.ExistsAsync<Role>(x =>
                            x.RoleGroupId == request.ParentId);
                        if (hasPermission)
                        {
                            throw new UserFriendlyException("变更的父分组不能包含授权项。");
                        }

                        // TODO： 如果父分组变更时，需要时间修改所有相关角色的分组路径字段。
                    }

                    dbGroup.Name = request.Name;
                    dbGroup.Desc = request.Desc;
                    dbGroup.ParentId = request.ParentId;

                    await db.UpdateOnlyAsync(dbGroup, onlyFields: x => new {x.Name, x.Desc, x.ParentId},
                        where: x => x.Id == request.Id);
                }
                else
                {
                    // 新增时，如果当前父分组下已有角色节点，则不能添加.
                    if (request.ParentId > 0)
                    {
                        var roleExists = await db.ExistsAsync<Role>(x => x.RoleGroupId == request.ParentId);
                        if (roleExists)
                        {
                            throw new UserFriendlyException("该父分组下已包含具体角色，不能再添加分组节点。");
                        }
                    }

                    using (var trans = db.OpenTransaction())
                    {
                        request.Id = (int) await db.InsertAsync(new RoleGroup
                        {
                            Name = request.Name,
                            Desc = request.Desc,
                            ParentId = request.ParentId,
                            HasSubGroup = false
                        });

                        // 修改父分组的标识
                        if (request.ParentId > 0)
                        {
                            parentGroup.HasSubGroup = true;

                            await db.UpdateOnlyAsync(parentGroup, onlyFields: x => new {x.HasSubGroup},
                                where: x => x.Id == request.ParentId);
                        }

                        trans.Commit();
                    }
                }

                return request.Id ?? 0;
            }
        }

        public async Task<long> DeleteRoleGroup(long id, string appKey)
        {
            using (var db = GetConnection())
            {
                var group = await db.SingleByIdAsync<RoleGroup>(id);
                if (group == null)
                {
                    throw new UserFriendlyException("角色分组不存在。");
                }

                // 下面有子分组或下面包含角色则不能删除
                var hasChild = await db.ExistsAsync<RoleGroup>(x => x.ParentId == id);
                if (hasChild)
                {
                    throw new UserFriendlyException("该角色分组包括子分组，不能删除。");
                }

                var hasRole = await db.ExistsAsync<Role>(x => x.RoleGroupId == id);
                if (hasRole)
                {
                    throw new UserFriendlyException("该分组包含角色，不能删除。");
                }

                var count = await db.DeleteAsync<RoleGroup>(x => x.Id == id);

                return count == 1 ? id : 0;
            }
        }

        public async Task<List<RoleResDto>> GetRoles(RoleQueryDto query)
        {
            using (var db = GetConnection())
            {
                var builder = db.From<Role>();
                var groups = new List<RoleGroup>();
                if (query.RoleGroupId.HasValue && query.RoleGroupId > 0L)
                {
                    var group = await db.SingleByIdAsync<RoleGroup>(query.RoleGroupId);
                    if (group == null)
                    {
                        throw new UserFriendlyException("角色分组不存在。");
                    }

                    // 如果不是包含角色的分组，则把所有下级及孙级的分组下的角色都显示出来。
                    if (group.HasSubGroup)
                    {
                        builder.Where(x => x.RoleGroupPath.Contains($",{query.RoleGroupId},"));
                    }
                    else
                    {
                        builder.Where(x => x.RoleGroupId == query.RoleGroupId);
                    }

                    groups.Add(group);
                }
                else
                {
                    groups = await db.SelectAsync<RoleGroup>(x => !x.HasSubGroup);

                    if (groups == null || groups.Count == 0)
                    {
                        return new List<RoleResDto>();
                    }

                    builder.Where(x => Sql.In(x.RoleGroupId, groups.Select(g => g.Id).ToList()));
                }

                if (!string.IsNullOrEmpty(query.SearchName))
                {
                    builder.Where(x => x.Name.Contains(query.SearchName));
                }

                var roles = await db.SelectAsync(builder);

                // 添加其它可能包含的分组
                var gIds = roles.Select(x => x.RoleGroupId).ToList();
                var includedIds = groups.Select(x => x.Id).ToList();
                var notIncludedIds = gIds.Except(includedIds).ToList();
                if (notIncludedIds.Count > 0)
                {
                    var otherGroups = await db.SelectAsync<RoleGroup>(x => Sql.In(x.Id, notIncludedIds));
                    otherGroups.ForEach(x => groups.Add(x));
                }

                // 设置角色的关联授权项id
                var rolePermissions = new List<RolePermission>();
                var ids = roles.Select(x => x.Id).ToList();
                if (ids.Count > 0)
                {
                    rolePermissions = await db.SelectAsync<RolePermission>(x => Sql.In(x.RoleId, ids));
                }

                return roles.Map(r =>
                {
                    var resDto = r.ConvertTo<RoleResDto>();

                    var group = groups.FirstOrDefault(x => x.Id == r.RoleGroupId);
                    resDto.GroupName = group?.Name;

                    resDto.PermissionIds = rolePermissions.Where(x => x.RoleId == r.Id).Select(x => x.PermissionId)
                        .ToHashSet();

                    return resDto;
                });
            }
        }

        public async Task<RoleResDto> GetRoleById(long id, string appKey)
        {
            using (var db = GetConnection())
            {
                var role = await db.SingleByIdAsync<Role>(id);
                if (role == null)
                {
                    throw new UserFriendlyException("角色不存在。");
                }

                var roleGroup = await db.SingleByIdAsync<RoleGroup>(role.RoleGroupId);
                if (roleGroup == null)
                {
                    return role.ConvertTo<RoleResDto>();
                }

                var roleRes = role.ConvertTo<RoleResDto>();
                roleRes.GroupName = roleGroup.Name;

                return roleRes;
            }
        }

        private async Task GenRoleGroupPathAsync(IDbConnection db, IList<long> groupIds, long roleGroupId)
        {
            groupIds.Insert(0, roleGroupId);

            var parentGroupId = await db.ScalarAsync<long>(
                db.From<RoleGroup>().Where(x => x.Id == roleGroupId).Select(x => x.ParentId));

            if (parentGroupId == 0L)
            {
                return;
            }

            await GenRoleGroupPathAsync(db, groupIds, parentGroupId);
        }

        public async Task<long> SaveRole(RoleSaveDto request)
        {
            var validationResult = await RoleValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            using (var db = GetConnection())
            {
                using (var trans = db.OpenTransaction())
                {
                    var group = await db.SingleAsync<RoleGroup>(x => x.Id == request.RoleGroupId);

                    if (group == null)
                    {
                        throw new UserFriendlyException("角色分组不存在。");
                    }

                    if (group.HasSubGroup)
                    {
                        throw new UserFriendlyException("角色只能挂在最下级角色分组下。");
                    }

                    var groupId = @group.Id;

                    var id = request.Id;
                    // 角色名称不能重复
                    var nameExists = id.HasValue && id > 0L
                        ? await db.ExistsAsync<Role>(x =>
                            x.RoleGroupId == groupId && x.Name == request.Name && x.Id != id.Value)
                        : await db.ExistsAsync<Role>(x => x.RoleGroupId == groupId && x.Name == request.Name);

                    if (nameExists)
                    {
                        throw new UserFriendlyException("同一分组下角色名称不能重复。");
                    }

                    var groupIds = new List<long>();
                    await GenRoleGroupPathAsync(db, groupIds, groupId);
                    var groupPath = $",{groupIds.Join(",")},";

                    if (id.HasValue && id > 0L)
                    {
                        // 先删除角色相关的授权项
                        await db.DeleteAsync<RolePermission>(x => x.RoleId == id);

                        await db.UpdateOnlyAsync(new Role
                            {
                                Id = id.Value,
                                Name = request.Name,
                                Desc = request.Desc,
                                RoleGroupId = groupId,
                                RoleGroupPath = groupPath,
                                ModifyTime = DateTime.Now
                            },
                            onlyFields: x => new {x.Name, x.Desc, x.RoleGroupId, x.RoleGroupPath, x.ModifyTime},
                            where: x => x.Id == id.Value);
                    }
                    else
                    {
                        id = await db.InsertAsync(new Role
                        {
                            Name = request.Name,
                            Desc = request.Desc,
                            RoleGroupId = groupId,
                            RoleGroupPath = groupPath,
                            CreateTime = DateTime.Now
                        }, true);
                    }

                    foreach (var permissionId in request.PermissionIds)
                    {
                        await db.InsertAsync(new RolePermission {RoleId = id.Value, PermissionId = permissionId});
                    }

                    trans.Commit();

                    return id ?? 0;
                }
            }
        }

        public async Task<long> DeleteRole(long id, string appKey)
        {
            using (var db = GetConnection())
            {
                var role = await db.SingleByIdAsync<Role>(id);
                if (role == null)
                {
                    throw new UserFriendlyException("角色不存在。");
                }

                var group = await db.SingleByIdAsync<RoleGroup>(role.RoleGroupId);
                if (group != null)
                {
                    throw new UserFriendlyException("无权操作。");
                }

                // 检查该角色下是否有用户，如果有则不能删除
                var hasUsers = await db.ExistsAsync<UserRole>(x => x.RoleId == id);
                if (hasUsers)
                {
                    throw new UserFriendlyException("当前权限下包含用户，不能删除。");
                }

                await db.DeleteByIdAsync<Role>(id);

                return id;
            }
        }

        public async Task<int> AssignRoles(AssignRoleSavDto request)
        {
            if (request.RoleIds == null)
            {
                throw new UserFriendlyException("角色id参加不能为null， 如果要清空当前用户的角色，则传递空数组。");
            }

            var count = 0;
            using (var db = GetConnection())
            {
                var user = await db.SingleByIdAsync<User>(request.UserId);
                if (user == null)
                {
                    throw new UserFriendlyException("用户不存在。");
                }

                using (var trans = db.OpenTransaction())
                {
                    await db.DeleteAsync<UserRole>(x => x.UserId == request.UserId);

                    foreach (var roleId in request.RoleIds)
                    {
                        var role = await db.SingleByIdAsync<Role>(roleId);
                        if (role == null)
                        {
                            throw new UserFriendlyException($"角色 {roleId} 不存在。");
                        }

                        var group = await db.SingleByIdAsync<RoleGroup>(role.RoleGroupId);
                        if (group == null)
                        {
                            throw new UserFriendlyException("角色所有分组不存在。");
                        }

                        await db.InsertAsync(new UserRole
                        {
                            UserId = request.UserId,
                            RoleId = roleId
                        });

                        count++;
                    }

                    trans.Commit();

                    return count;
                }
            }
        }

        private async Task GenPermissionGroupsAsync(
            IDbConnection db,
            Node<PermissionGroupResDto> node,
            ICollection<PermissionGroupResDto> leafGroups,
            long parentId)
        {
            var groups = await db.SelectAsync<PermissionGroup>(x => x.ParentId == parentId);
            foreach (var roleGroup in groups)
            {
                var resDto = roleGroup.ConvertTo<PermissionGroupResDto>();

                if (!roleGroup.HasSubGroup)
                {
                    leafGroups.Add(resDto);
                }

                var lastNode = node.AddChild(resDto);
                await GenPermissionGroupsAsync(db, lastNode, leafGroups, roleGroup.Id);
            }
        }

        public async Task<Node<PermissionGroupResDto>> GetPermissionGroups(PermissionGroupQueryDto query)
        {
            var root = new Node<PermissionGroupResDto>(new PermissionGroupResDto
            {
                Id = 0L,
                ParentId = 0L,
                Name = "全部"
            });

            var leafGroups = new List<PermissionGroupResDto>();
            using (var db = GetConnection())
            {
                await GenPermissionGroupsAsync(db, root, leafGroups, 0L);

                if (query.WithPermission && leafGroups.Count > 0)
                {
                    var gIds = leafGroups.Select(x => x.Id).ToList();
                    var permissions = await db.SelectAsync<Permission>(x => Sql.In(x.PermissionGroupId, gIds));
                    leafGroups.ForEach(g =>
                    {
                        g.Permissions = permissions.Where(x => x.PermissionGroupId == g.Id)
                            .Map(x => x.ConvertTo<PermissionResDto>());
                    });
                }
            }

            return root;
        }

        public async Task<long> SavePermissionGroup(PermissionGroupSaveDto request)
        {
//            var validationResult = await PermissionGroupValidator.ValidateAsync(request, ValidatorSelector);
//            if (!validationResult.IsValid)
//            {
//                throw new ValidationException(validationResult.Errors);
//            }

            using (var db = GetConnection())
            {
                var isEdit = request.Id.HasValue && request.Id > 0L;

                // 同一个分组下，名称不能重复。
                var nameExist = isEdit
                    ? await db.ExistsAsync<PermissionGroup>(x => x.Id != request.Id.Value &&
                                                                 x.ParentId == request.ParentId &&
                                                                 x.Name == request.Name)
                    : await db.ExistsAsync<PermissionGroup>(x => x.ParentId == request.ParentId &&
                                                                 x.Name == request.Name);

                if (nameExist)
                {
                    throw new UserFriendlyException($"角色分组: {request.Name}，已存在，请更换名称。");
                }

                // 父分组不能变更。 只有最下层的分组才能添加挂接具体授权项
                var parentGroup = await db.SingleByIdAsync<PermissionGroup>(request.ParentId);
                if (request.ParentId > 0)
                {
                    if (parentGroup == null)
                    {
                        throw new UserFriendlyException("父分组不存在。");
                    }
                }

                if (isEdit)
                {
                    var dbGroup = await db.SingleByIdAsync<PermissionGroup>(request.Id);
                    if (dbGroup == null)
                    {
                        throw new UserFriendlyException("权限分组不存在。");
                    }

                    if (dbGroup.ParentId != request.ParentId)
                    {
                        // 变更父分组时，如果父分组下包含授权项，则不能变更。因为分组下只能全部是子分组或授权项。
                        var hasPermission = await db.ExistsAsync<Permission>(x =>
                            x.PermissionGroupId == request.ParentId);
                        if (hasPermission)
                        {
                            throw new UserFriendlyException("变更的父分组不能包含授权项。");
                        }
                    }

                    dbGroup.Name = request.Name;
                    dbGroup.Desc = request.Desc;
                    dbGroup.ParentId = request.ParentId;

                    await db.UpdateOnlyAsync(dbGroup, onlyFields: x => new {x.Name, x.Desc, x.ParentId},
                        where: x => x.Id == request.Id);
                }
                else
                {
                    // 新增时，如果当前父分组下已有授权项节点，则不能添加.
                    if (request.ParentId > 0)
                    {
                        var permissionExists = await db.ExistsAsync<Permission>(x =>
                            x.PermissionGroupId == request.ParentId);
                        if (permissionExists)
                        {
                            throw new UserFriendlyException("该父分组下已包含具体授权项，不能再添加分组节点。");
                        }
                    }

                    using (var trans = db.OpenTransaction())
                    {
                        request.Id = (int) await db.InsertAsync(new PermissionGroup
                        {
                            Name = request.Name,
                            Desc = request.Desc,
                            ParentId = request.ParentId,
                            HasSubGroup = false
                        });

                        // 修改父分组的标识
                        if (request.ParentId > 0)
                        {
                            parentGroup.HasSubGroup = true;

                            await db.UpdateOnlyAsync(parentGroup, onlyFields: x => new {x.HasSubGroup},
                                where: x => x.Id == request.ParentId);
                        }

                        trans.Commit();
                    }
                }

                return request.Id ?? 0;
            }
        }

        public async Task<long> DeletePermissionGroup(long id, string appKey)
        {
            using (var db = GetConnection())
            {
                var group = await db.SingleByIdAsync<PermissionGroup>(id);

                if (group == null)
                {
                    throw new UserFriendlyException("分组不存在。");
                }

                var hasSubGroup = await db.ExistsAsync<PermissionGroup>(x => x.ParentId == id);
                if (hasSubGroup)
                {
                    throw new UserFriendlyException("该分组下还有子分组，不能删除。");
                }

                await db.DeleteByIdAsync<PermissionGroup>(id);

                var hasPermission = await db.ExistsAsync<Permission>(x => x.PermissionGroupId == id);
                if (hasPermission)
                {
                    await db.DeleteAsync<Permission>(q => q.PermissionGroupId == group.Id);
                }

                return id;
            }
        }

        public async Task<PagedResult<PermissionResDto>> GetPermissions(PermissionQueryDto query)
        {
            var userId = query.UserId ?? 0L;
            var permissionGroupId = query.PermissionGroupId ?? 0L;

            var roleIds = new HashSet<long>();
            using (var db = GetConnection())
            {
                if (userId > 0L)
                {
                    roleIds = await db.ColumnDistinctAsync<long>(db.From<UserRole>()
                        .Where(x => x.UserId == userId)
                        .Select(x => x.RoleId));
                }

                var builder = db.From<Permission>();
                if (permissionGroupId > 0L)
                {
                    var group = await db.SingleByIdAsync<PermissionGroup>(permissionGroupId);
                    if (group == null)
                    {
                        throw new UserFriendlyException("授权分组不存在。");
                    }

                    if (group.HasSubGroup)
                    {
                        builder.Where(x => x.PermissionGroupPath.Contains($",{permissionGroupId},"));
                    }
                    else
                    {
                        builder.Where(x => x.PermissionGroupId == permissionGroupId);
                    }
                }

                if (roleIds.Count > 0)
                {
                    builder.Join<RolePermission>((p, rp) => p.Id == rp.PermissionId && Sql.In(rp.RoleId, roleIds));
                }

                if (!string.IsNullOrEmpty(query.SearchName))
                {
                    builder.Where(x => x.Code.Contains(query.SearchName) || x.Name.Contains(query.SearchName));
                }

                var total = await db.CountAsync(builder);
                // 当按用户查询时，用户可能包含多个角色，授权项可能重复。
                var permissions = (await db.SelectAsync(builder)).Distinct().ToList();
                if (permissions.Count == 0)
                {
                    return new PagedResult<PermissionResDto>
                    {
                        Results = new List<PermissionResDto>(),
                        PageIndex = query.PageIndex,
                        PageSize = query.PageSize,
                        Total = total
                    };
                }

                var groupIds = permissions.Select(x => x.PermissionGroupId).ToList();
                var groups = await db.SelectAsync<PermissionGroup>(x => Sql.In(x.Id, groupIds));

                var resDtos = new List<PermissionResDto>(permissions.Count);
                permissions.ForEach(perm =>
                {
                    var resDto = perm.ConvertTo<PermissionResDto>();
                    var group = groups.FirstOrDefault(x => x.Id == perm.PermissionGroupId);
                    if (group != null)
                    {
                        resDto.PermissionGroupId = group.Id;
                        resDto.PermissionGroupName = group.Name;
                    }

                    resDtos.Add(resDto);
                });

                return new PagedResult<PermissionResDto>
                {
                    Results = resDtos,
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    Total = total
                };
            }
        }

        public async Task<PermissionResDto> GetPermissionById(long id, string appKey)
        {
            using (var db = GetConnection())
            {
                var permission = await db.SingleByIdAsync<Permission>(id);
                if (permission == null)
                {
                    throw new UserFriendlyException("权限信息不存在.");
                }

                var group = await db.SingleByIdAsync<PermissionGroup>(permission.PermissionGroupId);
                if (group == null)
                {
                    throw new UserFriendlyException("权限分组不存在。");
                }

                var resDto = permission.ConvertTo<PermissionResDto>();
                resDto.PermissionGroupId = group.Id;
                resDto.PermissionGroupName = group.Name;

                return resDto;
            }
        }

        private async Task GenPermissionGroupPathAsync(IDbConnection db, IList<long> groupIds, long groupId)
        {
            groupIds.Insert(0, groupId);

            var parentGroupId = await db.ScalarAsync<long>(
                db.From<PermissionGroup>().Where(x => x.Id == groupId).Select(x => x.ParentId));

            if (parentGroupId == 0L)
            {
                return;
            }

            await GenPermissionGroupPathAsync(db, groupIds, parentGroupId);
        }

        public async Task<long> SavePermission(PermissionSaveDto request)
        {
//            var validationResult = await PermissionValidator.ValidateAsync(request, ValidatorSelector);
//            if (!validationResult.IsValid)
//            {
//                throw new ValidationException(validationResult.Errors);
//            }

            using (var db = GetConnection())
            {
                var group = await db.SingleByIdAsync<PermissionGroup>(request.PermissionGroupId);
                if (group == null)
                {
                    throw new UserFriendlyException("授权分组不存在。");
                }

                var isEdit = request.Id.HasValue && request.Id > 0L;

                // 同一分组下编码不能重复
                var codeExist = isEdit
                    ? await db.ExistsAsync<Permission>(x => x.PermissionGroupId == request.PermissionGroupId
                                                            && x.Id != request.Id
                                                            && x.Code == request.Code)
                    : await db.ExistsAsync<Permission>(x => x.PermissionGroupId == request.PermissionGroupId
                                                            && x.Code == request.Code);

                if (codeExist)
                {
                    throw new UserFriendlyException("同一授权分组下的授权代码不能为空。");
                }

                var id = request.Id ?? 0L;
                using (var trans = db.OpenTransaction())
                {
                    var groupIds = new List<long>();
                    await GenPermissionGroupPathAsync(db, groupIds, request.PermissionGroupId);
                    var groupPath = $",{groupIds.Join(",")},";

                    if (isEdit)
                    {
                        var dbPerm = await db.SingleByIdAsync<Permission>(request.Id);
                        if (dbPerm == null)
                        {
                            throw new UserFriendlyException("要更新的授权信息不存在。");
                        }

                        dbPerm.PermissionGroupId = request.PermissionGroupId;
                        dbPerm.Code = request.Code;
                        dbPerm.Name = request.Name;
                        dbPerm.Desc = request.Desc;
                        dbPerm.Meta = request.Meta;
                        dbPerm.PermissionGroupPath = groupPath;

                        await db.UpdateOnlyAsync(dbPerm,
                            onlyFields: x => new
                            {
                                x.Code, x.Name, x.Desc, x.Meta, x.PermissionGroupId, x.PermissionGroupPath
                            },
                            where: x => x.Id == dbPerm.Id);
                    }
                    else
                    {
                        id = await db.InsertAsync(new Permission
                        {
                            Code = request.Code,
                            Name = request.Name,
                            Desc = request.Desc,
                            PermissionGroupId = request.PermissionGroupId,
                            PermissionGroupPath = groupPath,
                            Meta = request.Meta
                        }, true);
                    }

                    trans.Commit();

                    return id;
                }
            }
        }

        public async Task<long> DeletePermission(long id, string appKey)
        {
            using (var db = GetConnection())
            {
                var permission = await db.SingleByIdAsync<Permission>(id);
                if (permission == null)
                {
                    throw new UserFriendlyException("授权信息不存在。");
                }

//                var group = await db.SingleByIdAsync<PermissionGroup>(permission.PermissionGroupId);
//                if (group != null)
//                {
//                    throw new UserFriendlyException("无权操作。");
//                }

                await db.DeleteByIdAsync<Permission>(id);

                return id;
            }
        }


        public async Task<List<DistrictResDto>> GetDistricts(DistrictQueryDto query)
        {
            using (var db = GetConnection())
            {
                return await db.SelectAsync<DistrictResDto>(db.From<District>()
                    .Where(x => x.ParentId == query.ParentId));
            }
        }

        public async Task<DistrictResDto> GetDistrictById(int id, string appKey)
        {
            using (var db = GetConnection())
            {
                var district = await db.SingleByIdAsync<DistrictResDto>(id);
                if (district == null)
                {
                    throw new UserFriendlyException("该行政区划不存在。");
                }

                return district.ConvertTo<DistrictResDto>();
            }
        }

        public async Task<DistrictResDto> GetDistrictByCode(string code, string appKey)
        {
            using (var db = GetConnection())
            {
                var district = await db.SingleAsync<District>(x => x.Code == code);
                if (district == null)
                {
                    throw new UserFriendlyException("该代码的行政区划不存在。");
                }

                return district.ConvertTo<DistrictResDto>();
            }
        }

        public async Task<int> SaveDistrict(DistrictSaveDto request)
        {
            using (var db = GetConnection())
            {
                // 新增时，父节点必须存在。同一级下的名称不能重复。
                var isEdit = request.Id.HasValue && request.Id > 0;

                // 父节点是否存在。
                if (request.ParentId > 0)
                {
                    var parent = await db.SingleAsync<District>(x => x.Id == request.ParentId);
                    if (parent == null)
                    {
                        throw new UserFriendlyException("父节点不存在。");
                    }
                }

                var nameExits = isEdit
                    ? await db.ExistsAsync<District>(x =>
                        x.ParentId == request.ParentId && x.Name == request.Name && x.Id != request.Id)
                    : await db.ExistsAsync<District>(x => x.ParentId == request.ParentId && x.Name == request.Name);

                if (nameExits)
                {
                    throw new UserFriendlyException($"名称：{request.Name} 已存在，请修改名称。");
                }

                var codeExits = isEdit
                    ? await db.ExistsAsync<District>(x =>
                        x.ParentId == request.ParentId && x.Code == request.Code && x.Id != request.Id)
                    : await db.ExistsAsync<District>(x => x.ParentId == request.ParentId && x.Code == request.Code);

                if (codeExits)
                {
                    throw new UserFriendlyException($"编码：{request.Code} 已存在，请修改编码。");
                }

                if (isEdit)
                {
                    // 编辑时不能更改父节点
                    var dbDistrict = await db.SingleByIdAsync<District>(request.Id);
                    if (dbDistrict == null)
                    {
                        throw new UserFriendlyException("要编辑的行政区划不存在。");
                    }

                    if (dbDistrict.ParentId != request.ParentId)
                    {
                        throw new UserFriendlyException("修改时不能更新父节点id。");
                    }
                }

                await db.SaveAsync(request);

                return request.Id ?? 0;
            }
        }

        public async Task<int> DeleteDistrict(int id, string appKey)
        {
            using (var db = GetConnection())
            {
                // 如果有下级行政区划，则不能删除。
                var hasChild = await db.ExistsAsync<District>(x => x.ParentId == id);
                if (hasChild)
                {
                    throw new UserFriendlyException("当前行政区划包含下级行政区划，不能删除。");
                }

                var r = await db.DeleteAsync<District>(x => x.Id == id);

                return r == 1 ? id : 0;
            }
        }
    }
}