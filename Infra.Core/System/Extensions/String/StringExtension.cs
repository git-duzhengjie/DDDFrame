﻿using Infra.Core.Extensions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace System
{

    /// <summary>
    /// 字符串扩展类
    /// </summary>
    public static class StringExtension
    {
        //public static string  this[int index]
        //{
        //    get
        //    {
        //        switch (index)
        //        {
        //            case 0:
        //                return LastNmae;
        //                break;
        //            case 1:
        //                return NextNmae;
        //                break;
        //            default:
        //                return null;
        //                break;
        //        }
        //    }
        //    set
        //    {

        //        switch (index)
        //        {
        //            case 0:
        //                LastNmae = value;
        //                break;
        //            case 1:
        //                NextNmae = value;
        //                break;
        //            default:
        //                throw new Exception("Erro");
        //                break;
        //        }
        //    }
        //}

        /// <summary>
        ///     A string extension method that query if '@this' is null or empty.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>true if null or empty, false if not.</returns>
        public static bool IsNullOrEmpty(this string @this) => string.IsNullOrEmpty(@this);

        /// <summary>
        ///     A string extension method that query if '@this' is not null and not empty.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>false if null or empty, true if not.</returns>
        public static bool IsNotNullOrEmpty(this string @this) => !string.IsNullOrEmpty(@this);

        /// <summary>
        ///     A string extension method that query if '@this' is null or whiteSpace.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>true if null or whiteSpace, false if not.</returns>
        public static bool IsNullOrWhiteSpace(this string @this) => string.IsNullOrWhiteSpace(@this);

        /// <summary>
        ///     A string extension method that query if '@this' is not null and not whiteSpace.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>false if null or whiteSpace, true if not.</returns>
        public static bool IsNotNullOrWhiteSpace(this string @this) => !string.IsNullOrWhiteSpace(@this);

        /// <summary>
        ///     Concatenates the elements of an object array, using the specified separator between each element.
        /// </summary>
        /// <param name="separator">
        ///     The string to use as a separator.  is included in the returned string only if  has more
        ///     than one element.
        /// </param>
        /// <param name="values">An array that contains the elements to concatenate.</param>
        /// <returns>
        ///     A string that consists of the elements of  delimited by the  string. If  is an empty array, the method
        ///     returns .
        /// </returns>
        public static string Join<T>(this string separator, IEnumerable<T> values) => string.Join(separator, values);

        /// <summary>
        ///     Indicates whether the specified regular expression finds a match in the specified input string.
        /// </summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
        public static bool IsMatch(this string input, string pattern) => Regex.IsMatch(input, pattern);

        /// <summary>
        ///     Indicates whether the specified regular expression finds a match in the specified input string, using the
        ///     specified matching options.
        /// </summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
        public static bool IsMatch(this string input, string pattern, RegexOptions options) => Regex.IsMatch(input, pattern, options);

        /// <summary>An IEnumerable&lt;string&gt; extension method that concatenates the given this.</summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>A string.</returns>
        public static string Concatenate(this IEnumerable<string> @this)
        {
            var sb = new StringBuilder();

            foreach (var s in @this)
            {
                sb.Append(s);
            }

            return sb.ToString();
        }

        /// <summary>An IEnumerable&lt;T&gt; extension method that concatenates.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="source">The source to act on.</param>
        /// <param name="func">The function.</param>
        /// <returns>A string.</returns>
        public static string Concatenate<T>(this IEnumerable<T> source, Func<T, string> func)
        {
            var sb = new StringBuilder();
            foreach (var item in source)
            {
                sb.Append(func(item));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     A string extension method that query if this object contains the given value.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="value">The value.</param>
        /// <returns>true if the value is in the string, false if not.</returns>
        public static bool Contains(this string @this, string value) => @this.IndexOf(value, StringComparison.Ordinal) != -1;

        /// <summary>
        ///     A string extension method that query if this object contains the given value.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns>true if the value is in the string, false if not.</returns>
        public static bool Contains(this string @this, string value, StringComparison comparisonType) => @this.IndexOf(value, comparisonType) != -1;

        /// <summary>
        ///     A string extension method that extracts this object.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A string.</returns>
        public static string Extract(this string @this, Func<char, bool> predicate) => new string(@this.ToCharArray().Where(predicate).ToArray());

        /// <summary>
        ///     A string extension method that removes the letter.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A string.</returns>
        public static string RemoveWhere(this string @this, Func<char, bool> predicate) => new string(@this.ToCharArray().Where(x => !predicate(x)).ToArray());

        /// <summary>
        ///     Replaces the format item in a specified string with the text equivalent of the value of a corresponding
        ///     object instance in a specified array.
        /// </summary>
        /// <param name="this">A string containing zero or more format items.</param>
        /// <param name="values">An object array containing zero or more objects to format.</param>
        /// <returns>
        ///     A copy of format in which the format items have been replaced by the string equivalent of the corresponding
        ///     instances of object in args.
        /// </returns>
        public static string FormatWith(this string @this, params object[] values) => string.Format(@this, values);

        /// <summary>
        ///     A string extension method that query if '@this' satisfy the specified pattern.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="pattern">The pattern to use. Use '*' as wildcard string.</param>
        /// <returns>true if '@this' satisfy the specified pattern, false if not.</returns>
        public static bool IsLike(this string @this, string pattern)
        {
            // Turn the pattern into regex pattern, and match the whole string with ^$
            var regexPattern = "^" + Regex.Escape(pattern) + "$";

            // Escape special character ?, #, *, [], and [!]
            regexPattern = regexPattern.Replace(@"\[!", "[^")
                .Replace(@"\[", "[")
                .Replace(@"\]", "]")
                .Replace(@"\?", ".")
                .Replace(@"\*", ".*")
                .Replace(@"\#", @"\d");

            return Regex.IsMatch(@this, regexPattern);
        }

#if NET6_0

        /// <summary>
        /// SafeSubstring
        /// </summary>
        /// <param name="this"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string SafeSubstring(this string @this, int startIndex)
        {
            if (startIndex < 0 || startIndex > @this.Length)
            {
                return string.Empty;
            }
            return @this[startIndex..];
        }

#endif

        /// <summary>
        /// SafeSubstring
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SafeSubstring(this string str, int startIndex, int length)
        {
            if (startIndex < 0 || startIndex >= str.Length || length < 0)
            {
                return string.Empty;
            }
            return str.Substring(startIndex, Math.Min(str.Length - startIndex, length));
        }

#if NET6_0

        /// <summary>
        /// Sub, not only substring but support for negative numbers
        /// </summary>
        /// <param name="this">string to be handled</param>
        /// <param name="startIndex">startIndex to substract</param>
        /// <returns>substring</returns>
        public static string Sub(this string @this, int startIndex)
        {
            if (startIndex >= 0)
            {
                return @this.SafeSubstring(startIndex);
            }
            if (Math.Abs(startIndex) > @this.Length)
            {
                return string.Empty;
            }
            return @this[(@this.Length + startIndex)..];
        }
#endif

        /// <summary>
        ///     A string extension method that repeats the string a specified number of times.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="repeatCount">Number of repeats.</param>
        /// <returns>The repeated string.</returns>
        public static string Repeat(this string @this, int repeatCount)
        {
            if (@this.Length == 1)
            {
                return new string(@this[0], repeatCount);
            }

            var sb = new StringBuilder(repeatCount * @this.Length);
            while (repeatCount-- > 0)
            {
                sb.Append(@this);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     A string extension method that reverses the given string.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>The string reversed.</returns>
        public static string Reverse(this string @this)
        {
            if (@this.Length <= 1)
            {
                return @this;
            }

            var chars = @this.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        ///     Returns a string array containing the substrings in this string that are delimited by elements of a specified
        ///     string array. A parameter specifies whether to return empty array elements.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="separator">A string that delimit the substrings in this string.</param>
        /// <param name="option">
        ///     (Optional) Specify RemoveEmptyEntries to omit empty array elements from the array returned,
        ///     or None to include empty array elements in the array returned.
        /// </param>
        /// <returns>
        ///     An array whose elements contain the substrings in this string that are delimited by the separator.
        /// </returns>
        public static string[] Split(this string @this, string separator, StringSplitOptions option = StringSplitOptions.None) => @this.Split(new[] { separator }, option);

        /// <summary>
        ///     A string extension method that converts the @this to a byte array.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as a byte[].</returns>
        public static byte[] TobyteArray(this string @this) => Encoding.UTF8.GetBytes(@this);

        /// <summary>
        ///     A string extension method that converts the @this to a byte array.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="encoding">encoding</param>
        /// <returns>@this as a byte[].</returns>
        public static byte[] TobyteArray(this string @this, Encoding encoding) => encoding.GetBytes(@this);

        public static byte[] GetBytes(this string str) => str.GetBytes(Encoding.UTF8);

        public static byte[] GetBytes(this string str, Encoding encoding) => encoding.GetBytes(str);

        /// <summary>
        ///     A string extension method that converts the @this to an enum.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as a T.</returns>
        public static T ToEnum<T>(this string @this) => (T)Enum.Parse(typeof(T), @this);

        /// <summary>
        ///     A string extension method that converts the @this to a title case.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as a string.</returns>
        public static string ToTitleCasethis(string @this) => new CultureInfo("en-US").TextInfo.ToTitleCase(@this);

        /// <summary>
        ///     A string extension method that converts the @this to a title case.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="cultureInfo">Information describing the culture.</param>
        /// <returns>@this as a string.</returns>
        public static string ToTitleCase(this string @this, CultureInfo cultureInfo) => cultureInfo.TextInfo.ToTitleCase(@this);

        /// <summary>
        ///     A string extension method that truncates.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>A string.</returns>
        public static string Truncate(this string @this, int maxLength) => @this.Truncate(maxLength, "...");

        /// <summary>
        ///     A string extension method that truncates.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>A string.</returns>
        public static string Truncate(this string @this, int maxLength, string suffix)
        {
            if (@this == null || @this.Length <= maxLength)
            {
                return @this;
            }
            return @this.Substring(0, maxLength - suffix.Length) + suffix;
        }

        /// <summary>
        /// EqualsIgnoreCase
        /// </summary>
        /// <param name="s1">string1</param>
        /// <param name="s2">string2</param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string s1, string s2)
            => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// string=>long
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static long? ToLong(this string @this)
        {
            bool status = long.TryParse(@this, out long result);

            if (status)
                return result;
            else
                return null;
        }

        public static string ToUTF8(this string @this)
        {
            byte[] bytes = Encoding.Default.GetBytes(@this);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string GetLast(this string @this, int length,char empty='0')
        {
            if (length >= @this.Length)
            {
                return @this.PadLeft(length,empty);
            }
            return @this.Substring(@this.Length - length);
        }

        public static int ToInt(this string @this)
        {
            return int.Parse(@this);
        }

        public static string PadYear(this string @this)
        {
            if (@this.EndsWith('年'))
            {
                @this = @this.TrimEnd('年');
            }
            if (@this.EndsWith('后'))
            {
                @this = @this.TrimEnd('后');
            }
            if (@this.Length == 2)
            {
                var now = DateTime.Now.Year.ToString().GetLast(2).ToInt();
                var year = @this.ToInt();
                if (year > now)
                {
                    return $"19{year:00}";
                }
                else
                {
                    return $"20{year:00}";
                }
            }
            return @this;
        }
    }
}
