using System;

namespace SummerRest.Utilities
{
    public static class StringExtensions
    {
        public static string FromFieldToUnityFieldName(this string fieldName)
        {
            Span<char> chars = stackalloc char[fieldName.Length];
            //Remove the first underscore
            var fieldNameWithoutUnderscore = fieldName[0] == '_' ? fieldName.AsSpan(1) : fieldName.AsSpan();
            fieldNameWithoutUnderscore.CopyTo(chars);
            chars = chars[..fieldNameWithoutUnderscore.Length];
            return chars.ToString();
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