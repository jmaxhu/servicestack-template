using System.Collections.Generic;
using MyApp.ServiceModel.Common;
using ServiceStack;

namespace MyApp.ServiceModel.User
{
    public class UserSaveDto : IDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [ApiMember(DataType = "integer", Format = "int64", Description = "用户的id")]
        public int? Id { get; set; }

        /// <summary>
        /// 应用appkey
        /// </summary>
        [ApiMember(IsRequired = true, DataType = "string", Description = "分配的应用key")]
        public string AppKey { get; set; }
        
        /// <summary>
        /// 用户名
        /// </summary>
        [ApiMember(IsRequired = true, DataType = "string", Description = "用户名, 一般是手机号")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [ApiMember(DataType = "string", Description = "密码")]
        public string Password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [ApiMember(DataType = "string", Description = "姓名")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [ApiMember(DataType = "string", Description = "用户状态")]
        public ValidStatus ValidStatus { get; set; }

        [ApiMember(Description = "在应用内用户管理时，设置该用户的相关角色")]
        public HashSet<long> RoleIds { get; set; }
        
        // TODO: 添加所在地区
    }
}