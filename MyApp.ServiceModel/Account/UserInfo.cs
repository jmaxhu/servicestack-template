using DayuCloud.Account.Model.User;
using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.Account
{
    /// <summary>
    /// 自定义的用户实体
    /// </summary>
    [Alias("User")]
    public class UserInfo : User
    {
        /// <summary>
        /// 用户相关联的组织id
        /// </summary>
        [Index]
        public long OrganizationId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Index]
        [StringLength(15)]
        public string Role { get; set; }
    }
}