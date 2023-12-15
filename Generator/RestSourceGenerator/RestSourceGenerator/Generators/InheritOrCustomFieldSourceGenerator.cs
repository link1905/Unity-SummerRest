using System.Text;
using Microsoft.CodeAnalysis;
using RestSourceGenerator.Metadata;
using SharedSourceGenerator.Data;
using SharedSourceGenerator.Generators;
using SharedSourceGenerator.Utilities;

namespace RestSourceGenerator.Generators
{
    [Generator]
    public class InheritOrCustomFieldSourceGenerator : AttributedFieldSourceGenerator
    {
        public override string AttributeDisplayName => ProjectReflection.Attributes.InheritOrCustom.FullName;
        protected override void Execute(GeneratorExecutionContext context, ClassOrStructFieldsData target)
        {
            var bodyBuilder = new StringBuilder();
            foreach (var fieldData in target.Fields)
            {
                var fieldSymbol = fieldData.Item1; 
                var propName = fieldSymbol.Name.FromFieldToPropName();
                var inheritCheck = $"{fieldSymbol.Name}InheritCheck";
                bodyBuilder.Append($@"
private bool {inheritCheck} = true;
public {fieldData.Item1.Type.ToDisplayString()} {propName} {{
    get {{
        if ({inheritCheck} && {DefaultPropertyNames.Parent} is not null)
            return {DefaultPropertyNames.Parent}.{propName};
        return {fieldSymbol.Name};
    }}
}}
");
            }
            context.GenerateFormattedCode(ProjectReflection.Attributes.InheritOrCustom.Name, target.Self, 
                bodyBuilder.ToString());
        }
    }
}