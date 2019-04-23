using ServiceStack;
using MyApp.ServiceModel.Common;

namespace MyApp.ServiceModel.Role
{
    /// <summary>
    /// 简单角色信息
    /// </summary>
    public class RoleSimpleResDto : IDto
    {
        [ApiMember(DataType = "integer", Format = "int64", Description = "角色id")]
        public long Id { get; set; }

        [ApiMember(DataType = "string", Description = "角色名称")]
        public string Name { get; set; }

        [ApiMember(DataType = "string", Description = "角色描述")]
        public string Desc { get; set; }
    }
}