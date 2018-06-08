using DayuCloud.Model.Common;

namespace MyApp.ServiceModel.Models
{
    /// <summary>
    /// 上传文件的返回信息
    /// </summary>
    public class FileResDto : IDto
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件路径, 完整的可在浏览器中访问的文件路径
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// 相对路径，用于保存。
        /// </summary>
        public string RelPath { get; set; }

        /// <summary>
        /// 文件大小。单位为 Byte
        /// </summary>
        public long Size { get; set; }
        
        /// <summary>
        /// 客户端上传时传递的额外参数，直接原样返回。
        /// </summary>
        public object Meta { get; set; }
    }
}