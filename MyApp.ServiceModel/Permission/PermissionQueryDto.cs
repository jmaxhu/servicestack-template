using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Permission
{
    public class PermissionQueryDto : PagedQuery
    {
        [ApiMember(IsRequired = true, DataType = "string", Description = "应用key")]
        public string AppKey { get; set; }

        [ApiMember(IsRequired = false, DataType = "integer", Description = "根据用户id来筛选该用户所在角色的所有授权列表")]
        public int? UserId { get; set; }

        [ApiMember(IsRequired = false, DataType = "integer", Format = "int64", Description = "根据授权分组id来筛选授权列表")]
        public long? PermissionGroupId { get; set; }
    }
}