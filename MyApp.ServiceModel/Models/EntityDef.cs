using System;
using System.Collections.Generic;

namespace MyApp.ServiceModel.Models
{
    /// <summary>
    /// 实体定义
    /// </summary>
    public class EntityDef
    {
        /// <summary>
        /// 名称 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 可能的基类
        /// </summary>
        public Type BaseType { get; set; } = null;

        /// <summary>
        /// 实体包含的属性列表
        /// </summary>
        public IEnumerable<EntityProperty> Properties { get; set; }
    }

    /// <summary>
    /// 实体属性
    /// </summary>
    public class EntityProperty
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type Type { get; set; }
    }
}