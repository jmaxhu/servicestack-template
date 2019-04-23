using MyApp.ServiceModel.Common;

namespace MyApp.ServiceModel.District
{
    /// <summary>
    /// 行政区划保存对象
    /// </summary>
    public class DistrictSaveDto : IDto
    {
        /// <summary>
        /// 应用key
        /// </summary>
        public string AppKey { get; set; }
        
        /// <summary>
        /// 自增编号
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 父级id
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 行政区划编码
        /// </summary>
        /// <remarks>参考： http://www.mca.gov.cn/article/sj/xzqh/</remarks>
        public string Code { get; set; }

        /// <summary>
        /// 名称，比如：浙江省、杭州市、上城区，可以是任一级别的地区名称，包括，省，市，县，乡，村等。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
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
        /// 简介
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhone { get; set; }
        
        /// <summary>
        /// 地区边界地址
        /// </summary>
        public string Boundary { get; set; }
    }
}