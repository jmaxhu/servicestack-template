using System.Collections.Generic;
using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Role
{
    public class RoleGroupResDto : IDto
    {
        [ApiMember(DataType = "integer", Format = "int64", Description = "角色分组id")]
        public long Id { get; set; }
        
        [ApiMember(DataType = "integer", Format = "int64", Description = "父分组id")]
        public long ParentId { get; set; }

        [ApiMember(DataType = "string", Description = "分组名称")]
        public string Name { get; set; }

        [ApiMember(DataType = "string", Description = "分组描述")]
        public string Desc { get; set; }
        
        [ApiMember(Description = "是否包含子分组")]        
        public bool HasSubGroup { get; set; }
        
        [ApiMember(Description = "分组下的角色列表")]
        public List<RoleSimpleResDto> Roles { get; set; }
    }
}