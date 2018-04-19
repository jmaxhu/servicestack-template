using System.Collections.Generic;
using MyApp.ServiceModel.Models;

namespace MyApp.ServiceModel.User
{
    public class UserSaveDto: IDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// 组织id
        /// </summary>
        public long OrganizationId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public UseStatus? UseStatus { get; set; }
    }
}