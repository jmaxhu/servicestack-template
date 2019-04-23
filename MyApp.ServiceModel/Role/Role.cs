using System;
using MyApp.ServiceModel.Common;
using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.Role
{
    /// <summary>
    /// 角色表（用于给应用提供统一的角色，权限服务)
    /// </summary>
    [Alias("sys_role")]
    public class Role : Entity
    {
        /// <summary>
        /// 自增id
        /// </summary>
        [AutoIncrement]
        [PrimaryKey]
        public long Id { get; set; }

        /// <summary>
        /// 角色分组id
        /// </summary>
        [Index]
        [References(typeof(RoleGroup))]
        public long RoleGroupId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [StringLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(500)]
        public string Desc { get; set; }

        /// <summary>
        /// 角色所在分组id路径，从根路径开始。以','分隔，头和尾也包含 ','.
        /// </summary>
        [StringLength(500)]
        public string RoleGroupPath { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
    }
}