using System.Data;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Caching;
using ServiceStack.Data;

namespace MyApp.Manage
{
    /// <summary>
    /// 业务服务类的基类
    /// </summary>
    public abstract class ManageBase
    {
        private ICacheClient _cache;
        private IDbConnectionFactory _dbFactory;

        protected virtual IDbConnectionFactory DbFactory =>
            _dbFactory ?? (_dbFactory = HostContext.AppHost.TryResolve<IDbConnectionFactory>());

        public virtual ICacheClient Cache => _cache ?? (_cache = HostContext.AppHost.GetCacheClient(null));


        /// <summary>
        /// 得到数据库链接
        /// </summary>
        protected virtual IDbConnection GetConnection()
        {
            return DbFactory.OpenDbConnection();
        }
    }
}