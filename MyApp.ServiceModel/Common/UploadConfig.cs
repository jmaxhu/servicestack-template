using System.Collections.Generic;
using System.Linq;

namespace MyApp.ServiceModel.Common
{
    /// <summary>
    /// 上传配置
    /// </summary>
    public class UploadConfig
    {
        /// <summary>
        /// 上传文件保存的路径
        /// </summary>
        public string UploadPath { get; set; }

        /// <summary>
        /// 允许上传的文件扩展名列表，以'，'分隔。
        /// </summary>
        public string AllowUploadExts { get; set; }

        private List<string> _allowExts;

        /// <summary>
        /// 以列表形式返回的有效扩展名。
        /// </summary>
        public List<string> AllowExts => _allowExts ?? (_allowExts = AllowUploadExts.Split(',').ToList());

        /// <summary>
        /// 上传最大允许字节
        /// </summary>
        public int MaxSize { get; set; }
    }
}