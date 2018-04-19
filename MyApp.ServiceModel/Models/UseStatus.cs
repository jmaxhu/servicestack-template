using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.Models
{
    /// <summary>
    /// 通用的使用状态
    /// </summary>
    [EnumAsInt]
    public enum UseStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        Valid = 1,

        /// <summary>
        /// 无效
        /// </summary>
        Invalid
    }
}