using System.Runtime.Serialization;
using ServiceStack;

namespace MyApp.ServiceModel.Common
{
    /// <summary>
    /// 通用的分页搜索参数
    /// </summary>
    public class PagedQuery
    {
        private int _pageSize;

        /// <summary>
        /// 通用的主要搜索项
        /// </summary>
        [ApiMember(DataType = "string", Description = "通用的搜索条件.")]
        public string SearchName { get; set; }

        /// <summary>
        /// 第几页，以1开始。
        /// </summary>
        [ApiMember(IsRequired = true, DataType = "integer", Description = "第几页, 以1开始.")]
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数量, 最大500.
        /// </summary>
        [ApiMember(IsRequired = true, DataType = "integer", Description = "每页记录数，最大500.")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 500 ? 500 : value;
        }

        /// <summary>
        /// offset，只读。 
        /// </summary>
        [IgnoreDataMember]
        public int Skip => (PageIndex - 1) * PageSize;
    }
}