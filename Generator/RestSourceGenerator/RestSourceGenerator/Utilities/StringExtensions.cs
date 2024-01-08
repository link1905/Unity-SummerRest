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