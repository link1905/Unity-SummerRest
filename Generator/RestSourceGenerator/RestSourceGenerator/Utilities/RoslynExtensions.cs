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
        public static string BuildSequentialValues<T>(this IEnumerable<T> @params, Func<T, int, string> builder, string separator = ", ")
        {
            var values = @params.Select(builder).Where(value => value is not null).ToArray();
            return BuildSequentialValues(values, separator);
        }
        public static string BuildSequentialValues(this IReadOnlyList<string> values, string separator = ", ")
        {
            var length = values.Count;
            return length switch
            {
                0 => "",
                1 => values.First(),
                _ => string.Join(separator, values)
            };
        }
    }
}