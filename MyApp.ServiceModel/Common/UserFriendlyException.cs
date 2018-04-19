using System;

namespace MyApp.ServiceModel.Common
{
    /// <summary>
    /// 用户友好的异常。会把信息返回给前端调用方。
    /// </summary>
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException(string message) : base(message)
        {
        }

        public UserFriendlyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}