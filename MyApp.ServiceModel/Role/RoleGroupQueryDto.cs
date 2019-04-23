using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Role
{
    public class RoleGroupQueryDto : IDto
    {
        [ApiMember(IsRequired = true, DataType = "string", Description = "应用key")]
        public string AppKey { get; set; }
        
        [ApiMember(IsRequired = false, Description = "返回结果是否包含具体角色")]
        public bool WithRoles { get; set; }
    }
}