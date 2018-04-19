using MyApp.ServiceModel.Common;
using MyApp.ServiceModel.Models;
using ServiceStack;

namespace MyApp.ServiceModel.User
{
    public class UserQueryDto : PagedQuery, IDto
    {
        /// <summary>
        /// 组织id
        /// </summary>
        [ApiMember(DataType = "long", Description = "用户所属的组织id.")]
        public long? OrganizationId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [ApiMember(DataType = "string", Description = "用户的角色.")]
        public string Role { get; set; }

        /// <summary>
        /// 是否有效.可能被禁用.
        /// </summary>
        [ApiMember(DataType = "bool", Description = "用户的状态.")]
        public bool? Valid { get; set; }
    }
}