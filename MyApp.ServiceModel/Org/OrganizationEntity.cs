using DayuCloud.Model.Common;
using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.Org
{
    /// <summary>
    /// 组织
    /// </summary>
    public class OrganizationEntity : Entity
    {
        /// <summary>
        /// 自增id
        /// </summary>
        [AutoIncrement]
        [PrimaryKey]
        public long Id { get; set; }

        /// <summary>
        /// 父组织id
        /// <remarks>暂只支持二级组织</remarks>
        /// </summary>
        [Index]
        public long ParentId { get; set; }
        
        /// <summary>
        /// 父组织名称
        /// </summary>
        [Ignore]
        public string ParentName { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        [StringLength(2, 20)]
        public string Name { get; set; }
    }
}