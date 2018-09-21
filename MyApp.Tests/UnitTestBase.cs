using System;
using System.Data;
using DayuCloud.Manage;
using MyApp.Manage;
using MyApp.ServiceInterface;
using MyApp.ServiceModel.Account;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Caching;
using ServiceStack.Configuration;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Testing;

namespace MyApp.Tests
{
    public class UnitTestBase
    {
        protected ServiceStackHost AppHost;
        protected IOrgManage OrgManage;
        protected IDbConnection Db;

        [OneTimeSetUp]
        public void BaseSetUp()
        {
            AppHost = new BasicAppHost(typeof(FileService).Assembly)
            {
                ConfigureAppHost = host =>
                {
                    OrmLiteConfig.ExecFilter = new LogExecFilter();

                    host.Plugins.Add(new AuthFeature(() => new AuthUserSession(),
                        new IAuthProvider[]
                        {
                            new CredentialsAuthProvider()
                        }));

                    host.AppSettings = new EnvironmentVariableSettings();
                },
                ConfigureContainer = container =>
                {
                    var dbFactory = new OrmLiteConnectionFactory(DataSeed.TestDbConnection, MySqlDialect.Provider);

                    container.Register<IDbConnectionFactory>(c => dbFactory);
                    container.Register<ICacheClient>(new MemoryCacheClient());
                    container.Register<IAuthRepository>(c =>
                        new OrmLiteAuthRepository<UserInfo, UserAuthDetails>(dbFactory)
                        {
                            UseDistinctRoleTables = true
                        });

                    container.RegisterAs<OrgManage, IOrgManage>();
                    container.Register<ISchemaManage>(c => new MysqlSchemaManage("MyApp_test_db"));
                }
            };

            AppHost.Init();

            OrgManage = AppHost.Resolve<IOrgManage>();
            Db = AppHost.GetDbConnection();

            DataSeed.InitDataTable(AppHost);
            DataSeed.Create(AppHost);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (Db != null && Db.State == ConnectionState.Open)
            {
                try
                {
                    Db.Close();
                }
                catch (Exception e)
                {
                    TestContext.WriteLine(e.Message);
                }
            }

            AppHost.Dispose();
        }
    }

    internal class LogExecFilter : OrmLiteExecFilter
    {
        public override T Exec<T>(IDbConnection dbConn, Func<IDbCommand, T> filter)
        {
            var result = base.Exec(dbConn, filter);

            var lastSql = dbConn.GetLastSql();
            Console.WriteLine(lastSql);

            return result;
        }
    }
}