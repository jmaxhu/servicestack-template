using System.Collections.Generic;
using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Role
{
    public class RoleSaveDto : IDto
    {
        [ApiMember(DataType = "integer", Format = "int64", Description = "更新时传递的角色id")]
        public long? Id { get; set; }

        [ApiMember(IsRequired = true, DataType = "string", Description = "应用的key")]
        public string AppKey { get; set; }

        [ApiMember(IsRequired = true, DataType = "integer", Format = "int64", Description = "角色分组id")]
        public long RoleGroupId { get; set; }

        [ApiMember(IsRequired = true, DataType = "string", Description = "角色名称")]
        public string Name { get; set; }

        [ApiMember(DataType = "string", Description = "角色描述")]
        public string Desc { get; set; }
        
        [ApiMember(Description = "角色关联的授权项id列表")]
        public HashSet<long> PermissionIds { get; set; }
    }
}