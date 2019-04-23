using System.Collections.Generic;
using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Role
{
    public class AssignRoleSavDto : IDto
    {
        [ApiMember(IsRequired = true, Description = "分配的应用 key")]
        public string AppKey { get; set; }

        [ApiMember(IsRequired = true, Description = "要分配角色的用户 id")]
        public int UserId { get; set; }

        [ApiMember(IsRequired = true, Description = "要分配的角色 id 列表")]
        public HashSet<long> RoleIds { get; set; }
    }
}