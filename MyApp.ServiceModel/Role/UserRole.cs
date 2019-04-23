using MyApp.ServiceModel.Common;
using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.Role
{
    /// <summary>
    /// 用户角色关系信息
    /// </summary>
    [Alias("sys_user_role")]
    public class UserRole: Entity
    {
        /// <summary>
        /// 自增 id
        /// </summary>
        [AutoIncrement]
        [PrimaryKey]
        public long Id { get; set; }
        
        /// <summary>
        /// 应用 id
        /// </summary>
        [Index]
        public long AppClientId { get; set; }

        /// <summary>
        /// 角色 id
        /// </summary>
        [Index]
        [References(typeof(Role))]
        public long RoleId { get; set; }

        /// <summary>
        /// 用户 id
        /// </summary>
        [Index]
        [References(typeof(User.User))]
        public int UserId { get; set; }
    }
}