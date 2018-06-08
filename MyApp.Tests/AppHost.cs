using DayuCloud.Manage;
using MyApp.Manage;
using MyApp.ServiceInterface;
using MyApp.ServiceModel.User;
using Funq;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Caching;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace MyApp.Tests
{
    public class AppHost : AppSelfHostBase
    {
        public AppHost() : base("Test", typeof(FileService).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;

            SetConfig(new HostConfig {DebugMode = true});

            Plugins.Add(new CorsFeature());

            var dbFactory = new OrmLiteConnectionFactory(DataSeed.TestDbConnection, MySqlDialect.Provider);

            container.Register<IDbConnectionFactory>(c => dbFactory);
            container.Register<ICacheClient>(new MemoryCacheClient());
            container.Register<IAuthRepository>(c => new OrmLiteAuthRepository<UserEntity, UserAuthDetails>(dbFactory));

            container.RegisterAs<UserManage, IUserManage>();
            container.RegisterAs<OrgManage, IOrgManage>();
            container.RegisterAs<ReflectionManage, IReflectionManage>();
            container.Register<ISchemaManage>(c => new MysqlSchemaManage("MyApp_test_db"));
        }
    }
}