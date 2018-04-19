using MyApp.ServiceModel.Common;
using MyApp.ServiceModel.Models;
using ServiceStack;

namespace MyApp.ServiceModel.Org
{
    /// <inheritdoc />
    /// <summary>
    /// 组织搜索条件
    /// </summary>
    public class OrgQuery : PagedQuery, IDto
    {
        /// <summary>
        /// 父组织的id
        /// </summary>
        [ApiMember(DataType = "long", Description = "父组织id, 如果值为0则取所有根组织.")]
        public long? ParentId { get; set; }
    }
}