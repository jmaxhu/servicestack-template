using System;
using DayuCloud.Common;
using MyApp.ServiceModel.Models;
using MyApp.ServiceModel.Org;

namespace MyApp.ServiceModel.User
{
    public class UserResDto: IDto
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
        /// 姓名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public OrganizationEntity Org { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public UseStatus? UseStatus { get; set; }

        /// <summary>
        /// 锁定日期
        /// </summary>
        public DateTime? LockedDate { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 最近修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// 无效密码登录次数
        /// </summary>
        public virtual int InvalidLoginAttempts { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public virtual DateTime? LastLoginAttempt { get; set; }
    }
}