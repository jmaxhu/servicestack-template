using ServiceStack.Auth;
using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.User
{
    /// <summary>
    /// 自定义的用户实体
    /// </summary>
    [Alias("sys_user")]
    public class User : UserAuth
    {
        /// <summary>
        /// 用户类型。用于区分管理员用户还是其它应用导入的用户。 应用导入的用户统一使用 other。
        /// </summary>
        [Index]
        [StringLength(15)]
        public UserType UserType { get; set; }
        
        /// <summary>
        /// 用户相关联的组织id
        /// </summary>
        [Index]
        public long OrgId { get; set; }
    }
}