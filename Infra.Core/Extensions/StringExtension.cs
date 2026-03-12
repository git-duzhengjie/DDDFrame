using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infra.Core.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// 移除字符串中的常见中英文标点符号
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>移除标点后的纯文本字符串</returns>
        public static string RemoveCommonPunctuation(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // 正则匹配所有常见中英文标点，替换为空
            string punctuationPattern = @"[！？。，、；：""''""（）()【】[]{}《》<>·@#￥%……&*（）——+-=～·`~!@#$%^&*()_\-+=<>?:""{|,./;'\[\]·]";
            return Regex.Replace(input, punctuationPattern, "");
        }
    }
}
