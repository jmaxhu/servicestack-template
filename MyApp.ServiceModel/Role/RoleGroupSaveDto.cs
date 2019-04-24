using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Role
{
    /// <summary>
    /// 角色分组保存dto
    /// </summary>
    public class RoleGroupSaveDto: IDto
    {
        [ApiMember(Description = "角色分组id")]
        public long? Id { get; set; }

        [ApiMember(Description = "父分组id，如果是根分组则值为0.")]
        public long ParentId { get; set; }

        [ApiMember(Description = "角色分组名称")]
        public string Name { get; set; }

        [ApiMember(Description = "角色分组描述")]
        public string Desc { get; set; } 
    }
}