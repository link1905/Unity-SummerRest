using System;
using System.Text;

namespace SummerRest.Runtime.Extensions
{
    internal static class StringExtensions
    {
        public static string RemoveEscapeChar(this string str)
        {
            var result = new StringBuilder();
            foreach (var ch in str)
            {
                if (ch is '\n' or '\r' or ' ' or '\t')
                    continue;
                result.Append(ch);
            }

            return result.ToString();
        }

        public static string NotNullValue(this string data)
        {
            return data ?? string.Empty;
        }
        public static bool SplitKeyValue(this ReadOnlySpan<char> str, 
            out ReadOnlySpan<char> key,
            out ReadOnlySpan<char> value, 
            char separator = '=')
        {
            var index = str.IndexOf(separator);
            if (index < 0)
            {
                key = value = str;
                return false;
            }
            key = str[..index];
            value = str[(index + 1)..];
            return true;
        }
    }
}