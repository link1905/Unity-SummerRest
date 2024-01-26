using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using RestSourceGenerator.Metadata;

namespace RestSourceGenerator.Utilities
{
    public static class RoslynExtensions
    {
        public static void GenerateFormattedCode(this GeneratorExecutionContext context,
            string name, string source)
        {
            context.AddSource($"{name}.{RoslynDefaultValues.PostFixScriptName}", 
                SourceText.From(source.FormatCode(), RoslynDefaultValues.DefaultEncoding));
        }
        public static INamedTypeSymbol? GetNamedTypeSymbol(this SyntaxNode typeDeclarationSyntax,
            Compilation compilation)
        {
            var semanticModel = compilation.GetSemanticModel(typeDeclarationSyntax.SyntaxTree);
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclarationSyntax);
            return typeSymbol as INamedTypeSymbol;
        }
        public static AttributeData? GetAttributeWithName(this INamedTypeSymbol namedTypeSymbol, string name)
        {
            foreach (var attributeData in namedTypeSymbol.GetAttributes())
            {
                if (attributeData.AttributeClass?.ToDisplayString() == name)
                    return attributeData;
            }
            return null;
        }
        public static string BuildSequentialValues<T>(this IEnumerable<T> @params, Func<T, string> builder, string separator = ", ")
        {
            return BuildSequentialValues(@params.Select(builder), separator);
        }
        public static string BuildSequentialValues(this IEnumerable<string> values, string separator = ", ")
        {
            var paramsArray = values.ToArray();
            var length = paramsArray.Length;
            return length switch
            {
                0 => "",
                1 => paramsArray.First(),
                _ => string.Join(separator, paramsArray)
            };
        }
    }
}