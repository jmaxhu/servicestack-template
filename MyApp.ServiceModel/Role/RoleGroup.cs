using MyApp.ServiceModel.Common;
using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.Role
{
    /// <summary>
    /// 角色分组
    /// </summary>
    [Alias("sys_role_group")]
    public class RoleGroup : Entity
    {
        /// <summary>
        /// 角色分组id
        /// </summary>
        [AutoIncrement]
        [PrimaryKey]
        public long Id { get; set; }

        /// <summary>
        /// 角色分组父id
        /// </summary>
        [Index]
        public long ParentId { get; set; }

        /// <summary>
        /// 角色分组名称
        /// </summary>
        [StringLength(2, 20)]
        public string Name { get; set; }

        /// <summary>
        /// 角色分组描述
        /// </summary>
        [StringLength(200)]
        public string Desc { get; set; }

        /// <summary>
        /// 是否包含子分组
        /// </summary>
        public bool HasSubGroup { get; set; }
    }
}