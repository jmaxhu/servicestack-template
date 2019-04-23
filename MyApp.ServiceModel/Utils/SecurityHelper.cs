using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MyApp.ServiceModel.Utils
{
    public static class SecurityHelper
    {
        /// <summary>
        /// 计算某字符串的md5 hash值。 
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>十六进制hash值</returns>
        public static string MD5Hash(this string input)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
//                return Encoding.UTF8.GetString(bytes);
                return ConvertToString(bytes);
            }
        }

        /// <summary>
        /// 计算某字符串的 hmac hash 值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="secret">密钥</param>
        /// <returns>十六进制hash值</returns>
        public static string HMACMD5Hash(this string input, string secret)
        {
            using (var hmac = new HMACMD5(Encoding.UTF8.GetBytes(secret)))
            {
                var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
//                return Encoding.UTF8.GetString(bytes);
                return ConvertToString(bytes);
            }
        }

        /// <summary>
        /// 把字节流转换成十六进制字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        private static string ConvertToString(IEnumerable<byte> bytes)
        {
            var result = new StringBuilder();
            foreach (var b in bytes)
            {
                result.Append(b.ToString("X2"));
            }

            return result.ToString();
        }
    }
}