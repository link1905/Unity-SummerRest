using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using RestSourceGenerator.Metadata;

namespace RestSourceGenerator.Utilities
{
    public static class StringExtensions
    {
        public static SourceText FormatSource(this string code)
        {
            var format = code.FormatCode();
            return SourceText.From(format, RoslynDefaultValues.DefaultEncoding);
        }

        public static string ToEmbeddedString(this string str)
        {
            return @$"""{str}""";
        }

        public static string ToClassName(this string value)
        {
            StringBuilder formattedName = new StringBuilder();
            bool capitalizeNextChar = true;
            foreach (char c in value)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    if (formattedName.Length == 0 && char.IsDigit(c))
                    {
                        // If the first character is a digit, prefix with an underscore
                        formattedName.Append('_');
                    }
                    if (capitalizeNextChar)
                    {
                        formattedName.Append(char.ToUpper(c));
                        capitalizeNextChar = false;
                    }
                    else
                    {
                        formattedName.Append(c);
                    }
                }
                else
                {
                    capitalizeNextChar = true;
                }
            }

            return formattedName.ToString();
        }

        public static string FormatCode(this StringBuilder builder, CancellationToken cancelToken = default)
        {
            return builder.ToString().FormatCode(cancelToken);
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
        
        public static string GetRefToPredefinedValue(this string value)
        {
            if (!string.IsNullOrEmpty(value) && TextToPredefinedValue.TryGetValue(value, out var reference))
                return reference;
            return value.ToEmbeddedString();
        }
        private static readonly Dictionary<string, string> TextToPredefinedValue = new(StringComparer.OrdinalIgnoreCase)
        {
            {"utf-8", ProjectReflection.SummerRest.Runtime.RequestComponents.ContentType.Encodings + ".Utf8"},
            {"utf-16", ProjectReflection.SummerRest.Runtime.RequestComponents.ContentType.Encodings + ".Utf16"},
            {"us-ascii", ProjectReflection.SummerRest.Runtime.RequestComponents.ContentType.Encodings + ".UsAscii"},
            {"application/json", ProjectReflection.SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Application + ".Json"},
            {"application/octet-stream", ProjectReflection.SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Application + ".Octet"},
            {"application/soap+xml", ProjectReflection.SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Application + ".Soap"},
            {"application/xml", ProjectReflection.SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Application + ".Xml"},
            {"multipart/form-data", ProjectReflection.SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Multipart + ".FormData"},
            {"multipart/mixed", ProjectReflection.SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Multipart + ".Mixed"},
            {"text/plain", ProjectReflection.SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Text + ".Plain"},
            {"text/html", ProjectReflection.SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Text + ".Html"},
            {"text/richtext", ProjectReflection.SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Text + ".RichText"},
        };
    }
}