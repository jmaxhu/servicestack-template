using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Permission
{
    public class PermissionGroupQueryDto : IDto
    {
        [ApiMember(IsRequired = true, DataType = "string", Description = "应用key")]
        public string AppKey { get; set; }
        
        [ApiMember(IsRequired = false, Description = "是否同时返回分组下的授权项。")]
        public bool WithPermission { get; set; }
    }
}