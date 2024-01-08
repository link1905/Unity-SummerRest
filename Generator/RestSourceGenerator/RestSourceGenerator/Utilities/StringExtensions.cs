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