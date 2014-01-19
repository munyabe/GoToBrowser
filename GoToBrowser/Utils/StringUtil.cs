using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GoToBrowser.Utils
{
    /// <summary>
    /// <see cref="string"/>に関するユーティリティクラスです。
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// 指定した文字列の書式項目を、指定した文字列に置換します。
        /// </summary>
        /// <param name="format">複合書式指定文字列</param>
        /// <param name="newValues">出現するすべての対象を置換する文字列</param>
        /// <returns>書式項目が<paramref name="newValues"/>の対応する文字列に置換された<paramref name="format"/>の文字列</returns>
        public static string Format(string format, IDictionary<string, string> newValues)
        {
            Guard.ArgumentNotNull(newValues, "newValues");

            if (string.IsNullOrWhiteSpace(format))
            {
                return string.Empty;
            }

            var regex = new Regex("{[^${}]*}");
            return regex.Replace(format, match =>
            {
                var from = match.Value.Substring(1, match.Length - 2);
                string to;
                return newValues.TryGetValue(from, out to) ? to : match.Value;
            });
        }

        /// <summary>
        /// 文字列から大文字のみを抽出します。
        /// </summary>
        /// <param name="value">値を抽出する文字列</param>
        /// <returns>抽出した大文字の文字列</returns>
        public static string GetUpperCases(string value)
        {
            var chars = value.Where(char.IsUpper);
            return string.Concat(chars);
        }
    }
}