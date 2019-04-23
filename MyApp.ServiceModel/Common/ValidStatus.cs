using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.Common
{
    /// <summary>
    /// 通用的使用状态
    /// </summary>
    [EnumAsInt]
    public enum ValidStatus
    {
        /// <summary>
        /// 未设置
        /// </summary>
        None = 0,

        /// <summary>
        /// 有效
        /// </summary>
        Valid,

        /// <summary>
        /// 无效
        /// </summary>
        Invalid
    }
}