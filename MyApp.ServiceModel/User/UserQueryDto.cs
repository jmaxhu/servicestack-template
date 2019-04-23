using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.User
{
    public class UserQueryDto : PagedQuery, IDto
    {
        [ApiMember(IsRequired = true, DataType = "string", Description = "应用客户端的唯一key")]
        public string AppKey { get; set; }

//        [ApiMember(IsRequired = false, DataType = "integer", Format = "int64", Description = "用户所在的角色id")]
//        public long? RoleId { get; set; }

        /// <summary>
        /// 是否有效.可能被禁用.
        /// </summary>
        [ApiMember(Description = "用户的状态.")]
        public ValidStatus ValidStatus { get; set; }
    }
}