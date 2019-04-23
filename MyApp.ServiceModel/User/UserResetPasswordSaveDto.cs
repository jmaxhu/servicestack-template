using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.User
{
    public class UserResetPasswordSaveDto : IDto
    {
        [ApiMember(IsRequired = true, DataType = "integer", Description = "用户id")]
        public int UserId { get; set; }

        [ApiMember(IsRequired = true, DataType = "string", Description = "应用key")]
        public string AppKey { get; set; }
        
        [ApiMember(IsRequired = true, Description = "重置的密码")]
        public string Password { get; set; }
    }
}