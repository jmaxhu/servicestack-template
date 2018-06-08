using System;
using System.Threading.Tasks;
using DayuCloud.Common;
using MyApp.ServiceModel.Models;
using MyApp.ServiceModel.Org;
using MyApp.ServiceModel.User;
using NUnit.Framework;
using ServiceStack.Auth;
using ServiceStack.OrmLite;

namespace MyApp.Tests.UnitTests
{
    [TestFixture]
    public class UserTest : UnitTestBase
    {
        [Test]
        public async Task Can_Get_User()
        {
            var orgId = await Db.ScalarAsync<long>(Db.From<UserEntity>().Select(x => Sql.Max(x.OrganizationId)));
            var userQuery = new UserQueryDto
            {
                PageIndex = 1,
                PageSize = 10,
                OrganizationId = orgId
            };
            var userCount = await Db.CountAsync<UserEntity>(x => x.OrganizationId == orgId);

            var result = await UserManage.GetUsers(userQuery);

            Assert.AreEqual(userCount, result.Total);

            userQuery.OrganizationId = null;
            userQuery.Role = RoleConstants.Admin;

            userCount = await Db.CountAsync<UserEntity>(x => x.Role == RoleConstants.Admin);

            result = await UserManage.GetUsers(userQuery);

            Assert.AreEqual(userCount, result.Total);

            userQuery.SearchName = "__xxxx__";
            result = await UserManage.GetUsers(userQuery);

            Assert.AreEqual(0, result.Total);

            userQuery.OrganizationId = null;
            userQuery.Role = null;
            userQuery.SearchName = null;
            userQuery.Valid = false;

            userCount = await Db.CountAsync<UserEntity>(x => x.LockedDate != null);
            result = await UserManage.GetUsers(userQuery);

            Assert.AreEqual(userCount, result.Total);
        }

        [Test]
        public async Task Can_Get_User_ById()
        {
            var maxUserId = await Db.ScalarAsync<long>(Db.From<UserEntity>().Select(x => Sql.Max(x.Id)));

            var user = await UserManage.GetUserById(maxUserId);

            Assert.NotNull(user);
        }

        [Test]
        public async Task Can_Save_User()
        {
            var orgId = await Db.ScalarAsync<long>(Db.From<OrganizationEntity>().Select(x => Sql.Max(x.Id)));

            var user = new UserSaveDto
            {
                DisplayName = "江善于",
                Role = RoleConstants.Admin,
                Password = "123456",
                OrganizationId = orgId
            };

            // invalid username
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                user.UserName = "a";
                await UserManage.SaveUser(user);
            });

            // invalid role
            Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                user.UserName = "13800001111";
                user.Role = "invalid role name";
                await UserManage.SaveUser(user);
            });

            // invalid password
            Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                user.Role = RoleConstants.Watcher;
                user.Password = "111";
                await UserManage.SaveUser(user);
            });

            // invalid orgid
            Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                user.Password = "123456";
                user.OrganizationId = 0;
                await UserManage.SaveUser(user);
            });

            user.OrganizationId = orgId;

            var userId = await UserManage.SaveUser(user);

            Assert.IsTrue(userId > 0);

            var dbUser = await Db.SingleByIdAsync<UserEntity>(userId);

            Assert.AreEqual(user.DisplayName, dbUser.DisplayName);
            Assert.AreEqual(user.UserName, dbUser.UserName);
            Assert.AreEqual(user.Role, dbUser.Role);
            Assert.AreEqual(user.OrganizationId, dbUser.OrganizationId);

            // 更新
            user.Id = userId;
            user.DisplayName = "郑和";

            await UserManage.SaveUser(user);

            dbUser = await Db.SingleByIdAsync<UserEntity>(userId);

            Assert.AreEqual(user.DisplayName, dbUser.DisplayName);
        }

        [Test]
        public async Task Can_ChangePassword()
        {
            var user = await Db.SingleByIdAsync<UserEntity>(1);

            Assert.IsTrue(user != null);

            // 旧密码不正确
            Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await UserManage.ChangePassword(user.Id, "0", "19839393");
            });

            await UserManage.ChangePassword(user.Id, "123@qwe", "123456");

            user = await Db.SingleByIdAsync<UserEntity>(user.Id);

            var pwdRight = user.VerifyPassword("123456", out var _);

            Assert.AreEqual(true, pwdRight);
        }

        [Test]
        public async Task Can_Delete_User()
        {
            var lastUserId = await Db.ScalarAsync<long>(Db.From<UserEntity>().Select(x => Sql.Max(x.Id)));

            await UserManage.DeleteUser(lastUserId);

            var user = await Db.SingleByIdAsync<UserEntity>(lastUserId);

            Assert.IsNull(user);
        }
    }
}