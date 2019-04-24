using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using MyApp.ServiceModel.Permission;
using MyApp.ServiceModel.Role;
using MyApp.Manage;
using MyApp.ServiceInterface;
using MyApp.ServiceModel.Org;
using Funq;
using MyApp.ServiceModel.District;
using MyApp.ServiceModel.User;
using ServiceStack;
using ServiceStack.Api.OpenApi;
using ServiceStack.Auth;
using ServiceStack.Caching;
using ServiceStack.Data;
using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using ServiceStack.Text;
using ServiceStack.Validation;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace MyApp
{
    public class AppHost : AppHostBase
    {
        public AppHost() : base("MyApp",
            typeof(FileService).Assembly
        )
        {
            LogManager.LogFactory = new NLogFactory();
        }

        public override void Configure(Container container)
        {
            JsConfig.TextCase = TextCase.CamelCase;

            var debugMode = AppSettings.Get(nameof(HostConfig.DebugMode), false);
            var hostConfig = new HostConfig
            {
                DebugMode = debugMode
            };

            var allowFiles = AppSettings.GetList("AllowFileExtensions").ToList();
            allowFiles.ForEach(ext => hostConfig.AllowFileExtensions.Add(ext));

            if (!debugMode)
            {
                hostConfig.EnableFeatures = Feature.All.Remove(Feature.Metadata);
            }
            else
            {
                Plugins.Add(new OpenApiFeature());
            }

            SetConfig(hostConfig);

            Plugins.Add(new CorsFeature(
                allowedHeaders: "Content-Type,Authorization"
            ));

            // 开启 auto query 特性
            Plugins.Add(new AutoQueryFeature
            {
                MaxLimit = 100,
                IncludeTotal = true,
                EnableAutoQueryViewer = false
            });

            Plugins.Add(new AuthFeature(() => new AuthUserSession(),
                new IAuthProvider[]
                {
                    new JwtAuthProvider(AppSettings)
                    {
                        AuthKeyBase64 = AppSettings.GetString("jwt.AuthKeyBase64"),
                        RequireSecureConnection = false,
                        ExpireTokensIn = TimeSpan.FromMinutes(AppSettings.Get<int>("jwt.ExpireTokensIn"))
                    },
                    new CustomCredentialsAuthProvider()
                })
            {
                // 以字母开头,可包含数字,字母和下划线.长度为:3 ~ 20
                ValidUserNameRegEx = new Regex(@"\w+[\d_\w]{3,20}", RegexOptions.Compiled),
                ServiceRoutes = new Dictionary<Type, string[]>
                {
                    {typeof(AuthenticateService), new[] {"/auth", "/auth/{provider}"}}
                }
            });

            // 注册验证功能
            Plugins.Add(new ValidationFeature());
            container.RegisterValidators(typeof(RoleGroupValidator).Assembly);

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
            container.Register<IAuthRepository>(c => new OrmLiteAuthRepository<User, UserAuthDetails>(dbFactory)
            {
                UseDistinctRoleTables = false
            });

            container.RegisterAs<LocalAccountManage, IAccountManage>();
            container.RegisterAs<OrgManage, IOrgManage>();
            container.RegisterAs<ReflectionManage, IReflectionManage>();
            container.Register<ISchemaManage>(c => new MysqlSchemaManage("MyApp_DB"));

            InitData(container);
        }

        public override void OnExceptionTypeFilter(Exception ex, ResponseStatus responseStatus)
        {
            var log = LogManager.GetLogger("Exception Handlers");
            log.Error($"{responseStatus.ToJson()}", ex);

            base.OnExceptionTypeFilter(ex, responseStatus);
        }

        private static void InitData(Container container)
        {
            var dbFactory = container.Resolve<IDbConnectionFactory>();
            using (var db = dbFactory.OpenDbConnection())
            {
                if (!db.TableExists<User>())
                {
                    container.Resolve<IAuthRepository>().InitSchema();
                    var authRepo =
                        (OrmLiteAuthRepository<User, UserAuthDetails>) container.Resolve<IAuthRepository>();
                    var adminUser = new User
                    {
                        UserName = "admin_user",
                        DisplayName = "管理员",
                        Email = "admin_user@dayu.com",
                        OrgId = 0,
                        UserType = UserType.Admin
                    };

                    authRepo.CreateUserAuth(adminUser, "123456@qwe");
                }

                db.CreateTableIfNotExists<Organization>();
                db.CreateTableIfNotExists<RoleGroup>();
                db.CreateTableIfNotExists<Role>();
                db.CreateTableIfNotExists<PermissionGroup>();
                db.CreateTableIfNotExists<Permission>();
                db.CreateTableIfNotExists<RolePermission>();
                db.CreateTableIfNotExists<UserRole>();
                db.CreateTableIfNotExists<District>();
            }
        }
    }
}