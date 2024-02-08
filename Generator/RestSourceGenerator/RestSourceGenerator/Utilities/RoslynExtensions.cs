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
        
        
        public static string BuildSequentialValues<T>(this IEnumerable<T> @params, Func<T, int, string> builder, 
            string separator = RoslynDefaultValues.Commas, string? end = null)
        {
            var values = @params.Select(builder).Where(value => value is not null).ToArray();
            if (string.IsNullOrEmpty(end))
                return BuildSequentialValues(values, separator);
            var result = BuildSequentialValues(values, separator);
            if (string.IsNullOrEmpty(result))
                return result;
            return result + end;
        }

        public static string BuildSequentialValues(this IReadOnlyList<string> values, string separator = RoslynDefaultValues.Commas)
        {
            var length = values.Count;
            return length switch
            {
                0 => string.Empty,
                1 => values.First(),
                _ => string.Join(separator, values)
            };
        }
        
        public static string BuildSequentialConstants(this IEnumerable<(string name, string value)> values, string type)
        {
            return values.BuildSequentialValues((tuple, _) => BuildConst(type, tuple.name, tuple.value),
                separator: RoslynDefaultValues.SemiColons,
                end: RoslynDefaultValues.SemiColons);
        }
        
        public static string BuildSequentialReadonly(this IEnumerable<(string name, string value)> values, string type)
        {
            return values.BuildSequentialValues((tuple, _) => BuildReadonly(type, tuple.name, tuple.value),
                separator: RoslynDefaultValues.SemiColons,
                end: RoslynDefaultValues.SemiColons);
        }
        public static string BuildConst(string type, string name, string value)
        {
            return $"public const {type} {name.ToClassName()} = {value}";
        }
        public static string BuildReadonly(string type, string name, string value)
        {
            return $"public static readonly {type} {name.ToClassName()} = {value}";
        }
        public static string BuildArray(this IEnumerable<string> values, string type)
        {
            var elements = values.BuildSequentialValues((v, _) => v);
            return $"new {type}[] {{{elements}}}";
        }
    }
}