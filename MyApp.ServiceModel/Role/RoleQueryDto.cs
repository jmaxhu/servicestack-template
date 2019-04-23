using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Role
{
    /// <summary>
    /// 角色查询dto
    /// </summary>
    public class RoleQueryDto : IDto
    {
        /// <summary>
        /// 应用key
        /// </summary>
        [ApiMember(IsRequired = true, DataType = "string", Description = "应用key")]
        public string AppKey { get; set; }

        /// <summary>
        /// 角色分组id
        /// </summary>
        [ApiMember(DataType = "integer", Format = "int64", Description = "角色分组id，返回该分组下的角色信息。")]
        public long? RoleGroupId { get; set; }

        /// <summary>
        /// 角色名称搜索
        /// </summary>
        [ApiMember(DataType = "string", Description = "角色名称搜索")]
        public string SearchName { get; set; }
    }
}