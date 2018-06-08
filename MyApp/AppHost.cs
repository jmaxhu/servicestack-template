using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;
using DayuCloud.Account.Manage;
using DayuCloud.Account.Service;
using DayuCloud.Manage;
using MyApp.Manage;
using MyApp.ServiceInterface;
using MyApp.ServiceModel.Models;
using MyApp.ServiceModel.Org;
using MyApp.ServiceModel.User;
using Funq;
using ServiceStack;
using ServiceStack.Api.Swagger;
using ServiceStack.Auth;
using ServiceStack.Caching;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace MyApp
{
    public class AppHost : AppHostBase
    {
        public AppHost() : base("MyApp", typeof(FileService).Assembly, typeof(AccountService).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;

            var debugMode = AppSettings.Get(nameof(HostConfig.DebugMode), false);
            var hostConfig = new HostConfig
            {
                DebugMode = debugMode
            };

            hostConfig.AllowFileExtensions.Add("zip");
            hostConfig.AllowFileExtensions.Add("doc");
            hostConfig.AllowFileExtensions.Add("docx");
            hostConfig.AllowFileExtensions.Add("xls");
            hostConfig.AllowFileExtensions.Add("xlsx");

            if (!debugMode)
            {
                hostConfig.EnableFeatures = Feature.All.Remove(Feature.Metadata);
            }
            else
            {
                Plugins.Add(new SwaggerFeature());
            }

            SetConfig(hostConfig);

            Plugins.Add(new CorsFeature(
                allowedHeaders: "Content-Type,Authorization"
            ));
            Plugins.Add(new AutoQueryFeature
            {
                MaxLimit = 100,
                IncludeTotal = true,
                EnableAutoQueryViewer = false
            }); // 开启 auto query 特性
            Plugins.Add(new AuthFeature(() => new AuthUserSession(),
                new IAuthProvider[]
                {
                    new JwtAuthProvider(AppSettings)
                    {
                        AuthKeyBase64 = AppSettings.GetString("jwt.AuthKeyBase64"),
                        RequireSecureConnection = false
                    },
                    new CustomCredentialsAuthProvider()
                })
            {
                // 以字母开头,可包含数字,字母和下划线.长度为:5 ~ 20
                ValidUserNameRegEx = new Regex(@"\w+[\d_\w]{4,20}", RegexOptions.Compiled),
                ServiceRoutes = new Dictionary<Type, string[]>
                {
                    {typeof(AuthenticateService), new[] {"/auth", "/auth/{provider}"}}
                }
            });

            // redis init
            var redisConnStr = $"redis://{AppSettings.Get<string>("RedisHost")}:{AppSettings.Get<string>("RedisPort")}";
            var redisManager = new RedisManagerPool(redisConnStr);
            container.Register<IRedisClientsManager>(c => redisManager);
            container.Register(c => redisManager.GetCacheClient());

            var dbHost = AppSettings.Get<string>("DBHost");
            var dbPort = AppSettings.Get<string>("DBPort");
            var dbUser = AppSettings.Get<string>("DBUser");
            var dbPassword = AppSettings.Get<string>("DBPassword");
            var dbParam = AppSettings.Get<string>("DBParam");
            var dbConnStr = $"Server={dbHost};Port={dbPort};Uid={dbUser};Pwd={dbPassword};{dbParam}";
            var dbFactory = new OrmLiteConnectionFactory(dbConnStr, MySqlDialect.Provider)
            {
                AutoDisposeConnection = true,
                ConnectionFilter = db => new ProfiledDbConnection((DbConnection) db, MiniProfiler.Current)
            };

            ErrorMessages.InvalidUsernameOrPassword = "无效的用户名或密码";
            ErrorMessages.IllegalUsername = "无效的用户名";
            ErrorMessages.NotAuthenticated = "未验证";

//            dbFactory.RegisterConnection();
            // TODO: 根据配置注册其它数据库.

            container.Register<IDbConnectionFactory>(c => dbFactory);
            container.Register<ICacheClient>(new MemoryCacheClient());
            container.Register<IAuthRepository>(c => new OrmLiteAuthRepository<UserEntity, UserAuthDetails>(dbFactory));

            container.RegisterAs<RemoteAccountManage, IAccountManage>();
            container.RegisterAs<RemoteSSOManager, ISSOManage>();
            container.RegisterAs<UserManage, IUserManage>();
            container.RegisterAs<OrgManage, IOrgManage>();
            container.RegisterAs<ReflectionManage, IReflectionManage>();
            container.Register<ISchemaManage>(c => new MysqlSchemaManage("MyApp_db"));

            InitData(container);
        }

        private static void InitData(Container container)
        {
            var dbFactory = container.Resolve<IDbConnectionFactory>();
            using (var db = dbFactory.OpenDbConnection())
            {
                if (!db.TableExists<UserEntity>())
                {
                    container.Resolve<IAuthRepository>().InitSchema();
                    var authRepo =
                        (OrmLiteAuthRepository<UserEntity, UserAuthDetails>) container.Resolve<IAuthRepository>();
                    var adminUser = new UserEntity
                    {
                        UserName = "admin_user",
                        DisplayName = "管理员",
                        Email = "admin_user@dayu.com",
                        OrganizationId = 0,
                        Role = RoleConstants.Admin
                    };

                    authRepo.CreateUserAuth(adminUser, "123456@qwe");
                }

                db.CreateTableIfNotExists<OrganizationEntity>();
            }
        }
    }
}