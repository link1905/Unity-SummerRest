using System;
using System.Text;

namespace SummerRest.Scripts.Utilities.Extensions
{
    public static class StringExtensions
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
        public static string ReplaceFromIndexWith(this string value, int index, string replace)
        {
            //index + 1 => ignore index
            var needLength = index + replace.Length;
            Span<char> result = stackalloc char[needLength];
            value.AsSpan()[..result.Length].CopyTo(result);
            replace.AsSpan().CopyTo(result[index..]);
            return new string(result);
        }
    }
}