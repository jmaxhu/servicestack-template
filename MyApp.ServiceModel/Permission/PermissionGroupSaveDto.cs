using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Permission
{
    /// <summary>
    /// 权限分组保存dto
    /// </summary>
    public class PermissionGroupSaveDto : IDto
    {
        [ApiMember(IsRequired = true, DataType = "string", Description = "应用key")]
        public string AppKey { get; set; }

        [ApiMember(IsRequired = true, DataType = "integer", Format = "int64", Description = "可能的分组id, 编辑时必须提供。")]
        public long? Id { get; set; }

        [ApiMember(IsRequired = false, DataType = "integer", Format = "int64", Description = "父分组id，如果=0，则创建根分组。")]
        public long ParentId { get; set; }

        [ApiMember(IsRequired = true, DataType = "string", Description = "分组名称")]
        public string Name { get; set; }
        
        [ApiMember(Description = "权限分组描述")]
        public string Desc { get; set; } 
    }
}