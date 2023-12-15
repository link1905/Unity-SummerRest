using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharedSourceGenerator.Data;
using SharedSourceGenerator.Metadata;
using SharedSourceGenerator.Utilities;

namespace SharedSourceGenerator.Generators
{
    public abstract class AttributedFieldSourceGenerator : BasePartialSourceGenerator<ClassOrStructFieldsData>
    {
        public abstract string AttributeDisplayName { get; }
        protected override ClassOrStructFieldsData? GetSemanticTargetForGeneration(SyntaxNode typeDeclarationSyntax,
            Compilation compilation)
        {
            var typedSymbol = typeDeclarationSyntax.GetNamedTypeSymbol(compilation);
            if (typedSymbol is null)
                return null;
            var attributes = typedSymbol
                .GetFieldsWithAttribute(AttributeDisplayName)
                .ToImmutableArray();
            return attributes.IsEmpty ? null : new ClassOrStructFieldsData(typedSymbol, attributes);
        }
    }
}