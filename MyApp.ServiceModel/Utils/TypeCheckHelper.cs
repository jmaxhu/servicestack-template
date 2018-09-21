using System.Text.RegularExpressions;

namespace MyApp.ServiceModel.Utils
{
    public static class TypeCheckHelper
    {
        private static readonly Regex AlphabetReg = new Regex("^[a-zA-Z]$", RegexOptions.Compiled);

        /// <summary>
        /// 字符串是否权包含字母（不区分大小写）
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>是或否</returns>
        public static bool IsAlphabet(this string input)
        {
            return AlphabetReg.IsMatch(input);
        }
    }
}