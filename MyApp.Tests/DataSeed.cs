using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyApp.Manage;
using MyApp.ServiceModel.Models;
using MyApp.ServiceModel.Org;
using MyApp.ServiceModel.User;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace MyApp.Tests
{
    /// <summary>
    /// 初始数据
    /// </summary>
    public static class DataSeed
    {
        /// <summary>
        /// 测试数据库名
        /// </summary>
        public const string TestDatabaseName = "dayu_datacenter_test";

        /// <summary>
        /// 测试数据库连接字符串
        /// </summary>
        public static string TestDbConnection => ServiceStackHost.Instance.AppSettings.GetString("TEST_ENV") == "Test"
            ? $"Server=mysql;Database={TestDatabaseName};Uid=root;Pwd=dcenter_123@qwe;SslMode=None;CharSet=utf8;"
            : $"Server=127.0.0.1;Port=13306;Database={TestDatabaseName};Uid=dcenter;Pwd=dcenter_123@qwe;SslMode=None;CharSet=utf8;";

        /// <summary>
        /// 初始化表结构
        /// </summary>
        public static void InitDataTable(ServiceStackHost appHost)
        {
            var dbFactory = appHost.Resolve<IDbConnectionFactory>();
            var schema = appHost.Resolve<ISchemaManage>();

            using (var db = dbFactory.OpenDbConnection())
            {
                db.DropTable<OrganizationEntity>();

                db.DropTable<UserAuthRole>();
                db.DropTable<UserAuthDetails>();
                db.DropTable<UserEntity>();

                //删除其它表
                Task.Run(async () =>
                {
                    var tables = await schema.GetTables(TestDatabaseName);
                    foreach (var table in tables)
                    {
                        await schema.DropTable(table, TestDatabaseName);
                    }
                }).Wait();

                ((OrmLiteAuthRepository<UserEntity, UserAuthDetails>) appHost.Resolve<IAuthRepository>())
                    .InitSchema();

                db.CreateTable<OrganizationEntity>();
            }
        }

        /// <summary>
        /// 创建初始数据
        /// </summary>
        public static void Create(ServiceStackHost appHost)
        {
            var dbFactory = appHost.Resolve<IDbConnectionFactory>();
            var authRepo = (OrmLiteAuthRepository<UserEntity, UserAuthDetails>) appHost.Resolve<IAuthRepository>();
            var schemaManage = appHost.Resolve<ISchemaManage>();

            var rand = new Random(DateTime.Now.Millisecond);
            var parentOrgCount = rand.Next(5, 10);
            var childOrgIds = new List<long>();

            using (var db = dbFactory.Open())
            {
                using (var trans = db.OpenTransaction())
                {
                    // 生成组织
                    for (var i = 0; i < parentOrgCount; i++)
                    {
                        var id = db.Insert(new OrganizationEntity {ParentId = 0, Name = $"父组织_{i}"}, true);
                        var childOrgCount = rand.Next(5, 20);
                        for (var j = 0; j < childOrgCount; j++)
                        {
                            var childId = db.Insert(new OrganizationEntity {ParentId = id, Name = $"子组织_{i}"}, true);
                            childOrgIds.Add(childId);
                        }
                    }

                    // 添加一些用户
                    var userCount = rand.Next(10, 50);
                    for (var i = 0; i < userCount; i++)
                    {
                        var user = new UserEntity
                        {
                            UserName = $"username_{i}",
                            Email = $"username_{i}@dayu.com",
                            OrganizationId = childOrgIds[rand.Next(0, childOrgIds.Count - 1)],
                            DisplayName = $"用户_{i}",
                            Role = i % 2 == 0 ? RoleConstants.Admin :
                                i % 3 == 0 ? RoleConstants.Watcher : RoleConstants.Operator
                        };
                        if (i % 4 == 0)
                        {
                            user.LockedDate = DateTime.Now;
                        }

                        authRepo.CreateUserAuth(user, "123@qwe");
                    }

                    trans.Commit();
                }
            }
        }
    }
}