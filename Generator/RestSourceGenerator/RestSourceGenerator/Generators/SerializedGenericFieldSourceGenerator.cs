using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using RestSourceGenerator.Metadata;
using SharedSourceGenerator.Data;
using SharedSourceGenerator.Generators;
using SharedSourceGenerator.Utilities;

namespace RestSourceGenerator.Generators
{
    public class SerializedGenericFieldSourceGenerator : AttributedFieldSourceGenerator
    {
        private readonly struct ProcessedAttributeData
        {
            public ImmutableArray<string> BaseTypes { get; }
            public int DefaultIndex { get; }
            public ProcessedAttributeData(ImmutableArray<string> baseTypes, int defaultIndex)
            {
                BaseTypes = baseTypes;
                DefaultIndex = defaultIndex;
            }
            public string BuildBaseTypes()
            {
                // Only 1 base type => no need more include types
                var include = BaseTypes.Length == 1 ? "" : 
                    $", IncludeTypes = new []{BaseTypes.BuildTypesOfArray()}";
                return $"{BaseTypes[0].BuildTypeof()}{include}";
            }
            public string BuildDefaultType()
            {
                return BaseTypes[DefaultIndex].BuildTypeof();
            }
        }

        private ProcessedAttributeData? ProcessAttribute(AttributeData processedAttributeData)
        {
            var baseTypes = processedAttributeData.ConstructorArguments[0].Values
                .Where(e => !e.IsNull)
                .Select(e => e.Value.ToString());
            var defaultIdx = (int)(processedAttributeData.ConstructorArguments[1].Value ?? 0);
            return new ProcessedAttributeData(baseTypes.ToImmutableArray(), defaultIdx);
        }
        protected override void Execute(GeneratorExecutionContext context, ClassOrStructFieldsData target)
        {
            var bodyBuilder = new StringBuilder();
            foreach (var (fieldSymbol, fieldAttributeData) in target.Fields)
            {
                var processedAttData = ProcessAttribute(fieldAttributeData);
                if (processedAttData is null)
                    continue;
                var unitySerializableFieldName = fieldSymbol.Name.FromFieldToUnityFieldName();
                var containerName = $"{unitySerializableFieldName}{ProjectReflection.Attributes.SerializedGenericField.Container}";
                var propName = fieldSymbol.Name.FromFieldToPropName();
                var containerClassName = $"{propName}{ProjectReflection.Attributes.SerializedGenericField.Container}";
                var fieldTypeName = fieldSymbol.Type.ToDisplayString();
                bodyBuilder.Append($@"
[SerializeField] private ValueContainer {containerName};
public {fieldSymbol.Type.ToDisplayString()} {propName} => {containerName}.{ProjectReflection.Attributes.SerializedGenericField.Value};
// public Type {propName}Type => {containerName}.{ProjectReflection.Attributes.SerializedGenericField.Type};

[Serializable]
public class {containerClassName} : InterfaceContainer<{fieldTypeName}>
{{
    [SerializeField, Inherits({processedAttData.Value.BuildBaseTypes()})] 
    private TypeReference typeRef = new({processedAttData.Value.BuildDefaultType()});
}}
");
            }
            context.GenerateFormattedCode(ProjectReflection.Attributes.SerializedGenericField.Name, target.Self, 
                bodyBuilder.ToString(), usingStatements: 
                @"""
using System;
using SummerRest.Scripts.Attributes;
using TypeReferences;
using UnityEngine;
 """ );
        }

        public override string AttributeDisplayName => ProjectReflection.Attributes.SerializedGenericField.FullName;
    }
}