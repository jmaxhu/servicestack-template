using System;
using MyApp.ServiceModel.Common;
using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.District
{
    /// <summary>
    /// 行政区划
    /// </summary>
    public class District : Entity
    {
        /// <summary>
        /// 自增编号
        /// </summary>
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// 父级id
        /// </summary>
        [Index]
        public int ParentId { get; set; }

        /// <summary>
        /// 行政区划编码
        /// </summary>
        /// <remarks>参考： http://www.mca.gov.cn/article/sj/xzqh/</remarks>
        [Index(unique: true)]
        [StringLength(15)]
        public string Code { get; set; }

        /// <summary>
        /// 名称，比如：浙江省、杭州市、上城区，可以是任一级别的地区名称，包括，省，市，县，乡，村等。
        /// </summary>
        [Index]
        [StringLength(15)]
        public string Name { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        [StringLength(15)]
        public string ShortName { get; set; }

        /// <summary>
        /// 行政区划类型
        /// </summary>
        public DistrictType DistrictType { get; set; }

        /// <summary>
        /// 纬度 
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// 地图缩放等级
        /// </summary>
        public int Zoom { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [StringLength(5000)]
        public string Introduction { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        [StringLength(100)]
        public string Address { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [StringLength(15)]
        public string ContactPhone { get; set; }
        
        /// <summary>
        /// 包含所有父级节点及当前节点id的连接字符串。如'122.2332.1212'
        /// </summary>
        [StringLength(1000)]
        public string FullId { get; set; }
        
        /// <summary>
        /// 包含所有父级节点及当前节点的名称的字符串，如'浙江省.杭州市.上城区'等。
        /// </summary>
        [StringLength(2000)]
        public string FullName { get; set; }
        
        /// <summary>
        /// 是否有子节点    
        /// </summary>
        public bool HasChild { get; set; }
        
        /// <summary>
        /// 地区边界地址
        /// </summary>
        [StringLength(500)]
        public string Boundary { get; set; }
        
        /// <summary>
        /// 地区天气代码
        /// </summary>
        [StringLength(20)]
        public string CityCode { get; set; }

    }
}