using System.Runtime.Serialization;
using ServiceStack;

namespace MyApp.ServiceModel.Common
{
    /// <summary>
    /// 通用的分页搜索参数
    /// </summary>
    public abstract class PagedQuery
    {
        /// <summary>
        /// 通用的主要搜索项
        /// </summary>
        [ApiMember(DataType = "string", Description = "通用的搜索条件.")]
        public string SearchName { get; set; }

        /// <summary>
        /// 第几页，以1开始。
        /// </summary>
        [ApiMember(IsRequired = true, DataType = "int", Description = "第几页, 以1开始.")]
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        [ApiMember(IsRequired = true, DataType = "int", Description = "每页记录数")]
        public int PageSize { get; set; }

        /// <summary>
        /// offset，只读。 
        /// </summary>
        [IgnoreDataMember]
        public int Offset => (PageIndex - 1) * PageSize; 
    }
}