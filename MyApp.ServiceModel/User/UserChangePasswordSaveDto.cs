using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.User
{
    /// <summary>
    /// 用户修改密码dto
    /// </summary>
    public class UserChangePasswordSaveDto : IDto
    {
        [ApiMember(IsRequired = true, DataType = "integer", Description = "用户id")]
        public int UserId { get; set; }

        [ApiMember(IsRequired = true, DataType = "string", Description = "应用key")]
        public string AppKey { get; set; }

        [ApiMember(IsRequired = true, DataType = "string", Description = "旧密码")]
        public string OldPassword { get; set; }

        [ApiMember(IsRequired = true, DataType = "string", Description = "新密码")]
        public string NewPassword { get; set; }
    }
}