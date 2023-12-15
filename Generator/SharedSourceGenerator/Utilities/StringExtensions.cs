using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using SharedSourceGenerator.Metadata;

namespace SharedSourceGenerator.Utilities
{
    public static class StringExtensions
    {
        public static string FromFieldToPropName(this string fieldName)
        {
            Span<char> chars = stackalloc char[fieldName.Length];
            //Remove the first underscore
            var fieldNameWithoutUnderscore = fieldName[0] == '_' ? fieldName.AsSpan(1) : fieldName.AsSpan();
            fieldNameWithoutUnderscore.CopyTo(chars);
            chars = chars.Slice(0, fieldNameWithoutUnderscore.Length);
            //Upper the first char
            chars[0] = char.ToUpper(chars[0]);
            return chars.ToString();
        }
        public static string FromFieldToUnityFieldName(this string fieldName)
        {
            Span<char> chars = stackalloc char[fieldName.Length];
            //Remove the first underscore
            var fieldNameWithoutUnderscore = fieldName[0] == '_' ? fieldName.AsSpan(1) : fieldName.AsSpan();
            fieldNameWithoutUnderscore.CopyTo(chars);
            chars = chars.Slice(0, fieldNameWithoutUnderscore.Length);
            return chars.ToString();
        }

        public static SourceText FormatSource(this string code)
        {
            var format = code.FormatCode();
            return SourceText.From(format, RoslynDefaultValues.DefaultEncoding);
        }

        public static string FormatCode(this string code, CancellationToken cancelToken = default)
        {
            return CSharpSyntaxTree.ParseText(code, cancellationToken: cancelToken)
                .GetRoot(cancelToken)
                .NormalizeWhitespace()
                .SyntaxTree
                .GetText(cancelToken)
                .ToString();
        }
    }
}