using ServiceStack;

namespace MyApp.ServiceModel.Common
{
    /// <summary>
    /// 常规返回结果
    /// </summary>
    /// <typeparam name="T">任何类型</typeparam>
    public class NormalResult<T>
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        [ApiMember(Description = "接口返回值")]
        public T Result { get; set; }
    }
}