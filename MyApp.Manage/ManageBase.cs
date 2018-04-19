using ServiceStack;
using ServiceStack.Caching;
using ServiceStack.Data;

namespace MyApp.Manage
{
    public abstract class ManageBase
    {
        private ICacheClient _cache;
        private IDbConnectionFactory _dbFactory;

        public virtual IDbConnectionFactory DbFactory =>
            _dbFactory ?? (_dbFactory = HostContext.AppHost.TryResolve<IDbConnectionFactory>());

        public virtual ICacheClient Cache => _cache ?? (_cache = HostContext.AppHost.GetCacheClient(null));
    }
}