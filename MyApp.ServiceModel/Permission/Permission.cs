using MyApp.ServiceModel.Common;
using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.Permission
{
    /// <summary>
    /// 角色授权信息
    /// </summary>
    [Alias("sys_permission")]
    public class Permission : Entity
    {
        /// <summary>
        /// 自增id
        /// </summary>
        [AutoIncrement]
        [PrimaryKey]
        public long Id { get; set; }

        /// <summary>
        /// 权限分组id
        /// </summary>
        [Index]
        public long PermissionGroupId { get; set; }

        /// <summary>
        /// 授权项代码（英文）
        /// </summary>
        [StringLength(100)]
        public string Code { get; set; }

        /// <summary>
        /// 授权项名称
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 授权项描述
        /// </summary>
        [StringLength(300)]
        public string Desc { get; set; }

        /// <summary>
        /// 额外的权限属性信息
        /// </summary>
        public string Meta { get; set; }
        
        /// <summary>
        /// 权限分组id路径
        /// </summary>
        [StringLength(500)]
        public string PermissionGroupPath { get; set; }
    }
}