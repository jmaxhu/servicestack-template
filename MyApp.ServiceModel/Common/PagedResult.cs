using System.Collections.Generic;

namespace MyApp.ServiceModel.Common
{
    public class PagedResult<T>
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 第几页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 返回分页结果集成
        /// </summary>
        public List<T> Results { get; set; }
    }
}