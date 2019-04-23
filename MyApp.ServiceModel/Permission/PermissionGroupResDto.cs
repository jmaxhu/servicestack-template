using System.Collections.Generic;
using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Permission
{
    public class PermissionGroupResDto : IDto
    {
        [ApiMember(Description = "分组id")]
        public long Id { get; set; }

        [ApiMember(Description = "分组名称")]
        public string Name { get; set; }

        [ApiMember(DataType = "integer", Format = "int64", Description = "父分组id")]
        public long ParentId { get; set; }

        [ApiMember(DataType = "string", Description = "分组描述")]
        public string Desc { get; set; }

        [ApiMember(Description = "是否包含子分组")]
        public bool HasSubGroup { get; set; }

        [ApiMember(Description = "该分组下的授权项列表，只有在请求参加中要求返回具体授权项时才赋值。")]
        public List<PermissionResDto> Permissions { get; set; }
    }
}