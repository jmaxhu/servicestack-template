using System.Collections.Generic;
using System.Threading.Tasks;
using MyApp.ServiceModel.Common;
using MyApp.ServiceModel.Org;

namespace MyApp.Manage
{
    /// <summary>
    /// 组织管理
    /// </summary>
    public interface IOrgManage
    {
        /// <summary>
        /// 得到所有根组织
        /// </summary>
        /// <returns>组织列表</returns>
        Task<List<OrganizationEntity>> GetRootOrganizations();

        /// <summary>
        /// 根据id得到组织
        /// </summary>
        /// <param name="id">组织id</param>
        /// <returns>组织或null</returns>
        Task<OrganizationEntity> GetOrganizationById(long id);

        /// <summary>
        /// 分页查询组织
        /// </summary>
        /// <param name="query">分页查询条件</param>
        /// <returns>分页组织列表</returns>
        Task<PagedResult<OrganizationEntity>> GetOrganizations(OrgQuery query);

        /// <summary>
        /// 保存一个组织信息
        /// </summary>
        /// <param name="organizationEntity">待保存的组织，如果 id > 0 表示更新，否则为新增。</param>
        /// <returns>保存后的组织信息id</returns>
        Task<long> SaveOrganization(OrganizationEntity organizationEntity);

        /// <summary>
        /// 根据组织id删除组织
        /// </summary>
        /// <param name="id">组织id</param>
        Task DeleteOrganization(long id);
    }
}