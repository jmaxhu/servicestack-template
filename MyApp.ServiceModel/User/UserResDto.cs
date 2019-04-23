using System;
using MyApp.ServiceModel.District;
using MyApp.ServiceModel.Common;

namespace MyApp.ServiceModel.User
{
    public class UserResDto : IDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType UserType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ValidStatus? ValidStatus { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 最近修改日期
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime? LastLoginAttempt { get; set; }

        /// <summary>
        /// 用户所在系统的所在地，地区信息
        /// </summary>
        public DistrictResDto District { get; set; }
    }
}