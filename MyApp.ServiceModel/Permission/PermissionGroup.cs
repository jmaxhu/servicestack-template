using MyApp.ServiceModel.Common;
using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.Permission
{
    /// <summary>
    /// 权限分组
    /// </summary>
    [Alias("sys_permission_group")]
    public class PermissionGroup : Entity
    {
        /// <summary>
        /// 自增id
        /// </summary>
        [AutoIncrement]
        [PrimaryKey]
        public long Id { get; set; }

        /// <summary>
        /// 父分组id
        /// </summary>
        [Index]
        public long ParentId { get; set; }

        /// <summary>
        /// 权限分组名称
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }
        
        /// <summary>
        /// 权限分组描述
        /// </summary>
        [StringLength(200)]
        public string Desc { get; set; }
        
        /// <summary>
        /// 是否包含子分组
        /// </summary>
        public bool HasSubGroup { get; set; }
    }
}