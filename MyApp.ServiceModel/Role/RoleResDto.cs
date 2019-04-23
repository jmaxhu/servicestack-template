using System;
using System.Collections.Generic;
using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Role
{
    public class RoleResDto : IDto
    {
        [ApiMember(DataType = "integer", Format = "int64", Description = "角色id")]
        public long Id { get; set; }

        [ApiMember(DataType = "string", Description = "角色名称")]
        public string Name { get; set; }

        [ApiMember(DataType = "string", Description = "角色描述")]
        public string Desc { get; set; }

        [ApiMember(Description = "角色分组id")] public long RoleGroupId { get; set; }

        [ApiMember(DataType = "string", Description = "角色分组名称")]
        public string GroupName { get; set; }
        
        [ApiMember(Description = "角色关联的授权项id列表")]
        public HashSet<long> PermissionIds { get; set; }

        [ApiMember(Description = "创建时间")]
        public DateTime? CreateTime { get; set; }

        [ApiMember(Description = "最近修改时间")]
        public DateTime? ModifyTime { get; set; }
    }
}