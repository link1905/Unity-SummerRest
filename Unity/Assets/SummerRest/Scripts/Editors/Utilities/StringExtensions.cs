using System;

namespace SummerRest.Editors.Utilities
{
    public static class StringExtensions
    {
        public static string ReplaceFromIndexWith(this string value, int index, string replace)
        {
            //index + 1 => ignore index
            var needLength = index + replace.Length;
            Span<char> result = stackalloc char[needLength];
            value.AsSpan().CopyTo(result);
            replace.AsSpan().CopyTo(result[index..]);
            return new string(result);
        }
    }
}