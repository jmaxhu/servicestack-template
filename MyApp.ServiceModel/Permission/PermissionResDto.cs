using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Permission
{
    public class PermissionResDto : IDto
    {
        /// <summary>
        /// 自增id
        /// </summary>
        [ApiMember(Description = "可填的授权项id")]
        public long Id { get; set; }

        /// <summary>
        /// 授权所在分组id
        /// </summary>
        [ApiMember(Description = "授权所在分组id")]
        public long PermissionGroupId { get; set; }

        /// <summary>
        /// 权限分组名称
        /// </summary>
        [ApiMember(Description = "授权所在的组名称")]
        public string PermissionGroupName { get; set; }

        /// <summary>
        /// 授权项代码（英文）
        /// </summary>
        [ApiMember(Description = "授权编码，由用户自定义")]
        public string Code { get; set; }

        /// <summary>
        /// 授权项名称
        /// </summary>
        [ApiMember(Description = "授权编码对应的名称")]
        public string Name { get; set; }

        /// <summary>
        /// 授权项描述
        /// </summary>
        [ApiMember(Description = "授权的描述信息")]
        public string Desc { get; set; }

        /// <summary>
        /// 额外的权限属性信息
        /// </summary>
        [ApiMember(Description = "该授权额外的属性信息")]
        public string Meta { get; set; }
    }
}