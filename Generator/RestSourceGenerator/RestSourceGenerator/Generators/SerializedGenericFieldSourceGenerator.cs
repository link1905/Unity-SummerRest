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
    [Generator]
    public class SerializedGenericFieldSourceGenerator : AttributedFieldSourceGenerator
    {
        public override string AttributeDisplayName => ProjectReflection.Attributes.SerializedGenericField.FullName;
        private readonly struct ProcessedAttributeData
        {
            public ImmutableArray<string> Types { get; }
            public string DefaultType { get; }
            public ProcessedAttributeData(ImmutableArray<string> types, string defaultType)
            {
                Types = types;
                DefaultType = defaultType;
            }
            public string BuildInherits()
            {
                return Types.BuildTypesOfArray();
            }
            public string BuildDefaultType()
            {
                return DefaultType.BuildTypeof();
            }
        }
        private ProcessedAttributeData? ProcessAttribute(AttributeData processedAttributeData)
        {
            var defaultType = processedAttributeData.ConstructorArguments[0].Value.ToString();
            var baseTypes = processedAttributeData.ConstructorArguments[1].Values
                .Where(e => !e.IsNull)
                .Select(e => e.Value.ToString());
            return new ProcessedAttributeData(baseTypes.ToImmutableArray(), defaultType);
        }
        protected override void Execute(GeneratorExecutionContext context, ClassOrStructFieldsData target)
        {
            var bodyBuilder = new StringBuilder();
            foreach (var (fieldSymbol, fieldAttributeData) in target.Fields)
            {
                var processedAttData = ProcessAttribute(fieldAttributeData);
                if (processedAttData is null)
                    continue;
                var fieldTypeSymbol = fieldSymbol.Type.ToDisplayString();
                var unitySerializableFieldName = fieldSymbol.Name.FromFieldToUnityFieldName();
                var containerName = $"{unitySerializableFieldName}{ProjectReflection.Attributes.SerializedGenericField.Container}";
                var propName = fieldSymbol.Name.FromFieldToPropName();
                var containerClassName = $"{propName}{ProjectReflection.Attributes.SerializedGenericField.Container}";
                var fieldTypeName = fieldSymbol.Type.ToDisplayString();
                bodyBuilder.Append($@"
[SerializeField] private ValueContainer {containerName};
public {fieldTypeSymbol} {propName} => {containerName}.{ProjectReflection.Attributes.SerializedGenericField.Value};
public Type {propName}Type => {containerName}.{ProjectReflection.Attributes.SerializedGenericField.Type};

[Serializable]
public class {containerClassName} : InterfaceContainer<{fieldTypeName}>
{{
    [SerializeField, Inherits({processedAttData.Value.BuildInherits()})] 
    private TypeReference typeRef = new({processedAttData.Value.BuildDefaultType()});
}}
");
            }
            context.GenerateFormattedCode(ProjectReflection.Attributes.SerializedGenericField.Name, target.Self, 
                bodyBuilder.ToString(), usingStatements: 
                @"
using System;
using SummerRest.Scripts.Attributes;
using SummerRest.Scripts.DataStructures;
using TypeReferences;
using UnityEngine;");
        }

    }
}