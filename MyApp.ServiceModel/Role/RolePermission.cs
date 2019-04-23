using MyApp.ServiceModel.Common;
using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.Role
{
    /// <summary>
    /// 角色权限关系
    /// </summary>
    [Alias("sys_role_permission")]
    public class RolePermission : Entity
    {
        /// <summary>
        /// 关系id
        /// </summary>
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// 角色id
        /// </summary>
        [Index]
        [References(typeof(Role))]
        public long RoleId { get; set; }

        /// <summary>
        /// 权限id
        /// </summary>
        [Index]
        [References(typeof(Permission.Permission))]
        public long PermissionId { get; set; }
    }
}