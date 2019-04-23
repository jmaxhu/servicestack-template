using System.Collections.Generic;
using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.Permission
{
    /// <summary>
    /// 授权项保存dto
    /// </summary>
    public class PermissionSaveDto : IDto
    {
        /// <summary>
        /// 应用key
        /// </summary>
        [ApiMember(IsRequired = true, DataType = "string", Description = "应用key")]
        public string AppKey { get; set; }

        /// <summary>
        /// 自增id
        /// </summary>
        [ApiMember(IsRequired = false, DataType = "integer", Format = "int64", Description = "可填的授权项id")]
        public long? Id { get; set; }

        /// <summary>
        /// 权限分组id
        /// </summary>
        [ApiMember(IsRequired = true, DataType = "integer", Format = "int64", Description = "授权所在的组id")]
        public long PermissionGroupId { get; set; }

        /// <summary>
        /// 授权项代码（英文）
        /// </summary>
        [ApiMember(IsRequired = true, DataType = "string", Description = "授权编码，由用户自定义")]
        public string Code { get; set; }

        /// <summary>
        /// 授权项名称
        /// </summary>
        [ApiMember(IsRequired = true, DataType = "string", Description = "授权编码对应的名称")]
        public string Name { get; set; }

        /// <summary>
        /// 授权项描述
        /// </summary>
        [ApiMember(DataType = "string", Description = "授权的描述信息")]
        public string Desc { get; set; }

        /// <summary>
        /// 额外的权限属性信息
        /// </summary>
        [ApiMember(IsRequired = false, DataType = "string", Description = "该授权额外的属性信息")]
        public string Meta { get; set; }
    }
}