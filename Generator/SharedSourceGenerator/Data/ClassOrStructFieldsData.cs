using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharedSourceGenerator.Data
{
    public class ClassOrStructFieldsData
    {
        public INamedTypeSymbol Self { get; }
        public ImmutableArray<(IFieldSymbol, AttributeData)> Fields { get; }

        public ClassOrStructFieldsData(INamedTypeSymbol self, ImmutableArray<(IFieldSymbol, AttributeData)> fields)
        {
            Self = self;
            Fields = fields;
        }
    }
}