using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SharedSourceGenerator.Metadata;

namespace SharedSourceGenerator.Utilities
{
    public static class RoslynExtensions
    {
        public static bool IsCopyableField(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.IsValueType ||
                   typeSymbol.ToDisplayString() == CSharpReservedNames.TypeString;
        }

        public static string BuildTypeof(this string type) => $"typeof({type})";
        public static string BuildTypesOfArray(this IEnumerable<string> values)
        {
            return values.Select(BuildTypeof).BuildSequentialValues();
        }
        
        public static void GenerateFormattedCode(this GeneratorExecutionContext context,
            string hintName, INamedTypeSymbol namedTypeSymbol, string body, string? usingStatements = null, string? inherit = null)
        {
            var source = namedTypeSymbol.GenerateBoundingPartialSource(body, usingStatements, inherit);
            context.GenerateFormattedCode($"{namedTypeSymbol.Name}.{hintName}", source);
        }

        private static string? GetModifer(this ISymbol namedTypeSymbol)
        {
            if (namedTypeSymbol.DeclaringSyntaxReferences[0].GetSyntax() 
                is not TypeDeclarationSyntax typeDeclarationSyntax)
                return null;
            foreach (var modifier in typeDeclarationSyntax.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        return CSharpReservedNames.Public;
                    case SyntaxKind.ProtectedKeyword:
                        return CSharpReservedNames.Protected;
                    case SyntaxKind.PrivateKeyword:
                        return CSharpReservedNames.Private;
                    case SyntaxKind.InternalKeyword:
                        return CSharpReservedNames.Internal;
                    default:
                        continue;
                }
            }
            return null;
        }

        public static string GenerateBoundingPartialSource(this INamedTypeSymbol namedTypeSymbol,
            string body, string? usingStatement = null, string? inherit = null)
        {
            var @namespace = namedTypeSymbol.ContainingNamespace.IsGlobalNamespace ? null : 
                namedTypeSymbol.ContainingNamespace.ToDisplayString();
            var modifier = namedTypeSymbol.GetModifer();
            if (modifier is null)
                throw new InvalidDataException($"Can not get modifer of {namedTypeSymbol.Name}");
            var inheritText = string.IsNullOrEmpty(inherit) ? "" : $": {inherit}";
            var start = @namespace is null ? "" : @$"
namespace {@namespace} 
{{";
            var end = @namespace is null ? "" : @"
}";
            var result = $@"
// Auto-generated
{usingStatement}
{start}
    {modifier} {CSharpReservedNames.Partial} {CSharpReservedNames.Class} {namedTypeSymbol.Name} {inheritText}
    {{
        {body}
    }}
{end}
";
            return result;
        }
        
        public static void GenerateFormattedCode(this GeneratorExecutionContext context,
            string name, string source)
        {
            context.AddSource($"{name}.{RoslynDefaultValues.PostFixScriptName}", 
                SourceText.From(source.FormatCode(), RoslynDefaultValues.DefaultEncoding));
        }

        public static bool IsPartialNode(this SyntaxNode node) //Get all class declartions
        {
            if (node is not ClassDeclarationSyntax or StructDeclarationSyntax)
                return false;
            if (node is not TypeDeclarationSyntax clasNode)
                return false;
            foreach (var modifier in clasNode.Modifiers)
            {
                if (modifier.Text == CSharpReservedNames.Partial)
                    return true;
            }

            return false;
        }

        public static bool IsClassOrStruct(this ITypeSymbol typeSymbol) =>
            typeSymbol.TypeKind is TypeKind.Class or TypeKind.Struct;

        public static (INamedTypeSymbol self, INamedTypeSymbol @interface)? InheritInterface(
            this ISymbol typeSymbol,
            string interfaceName, bool includeParents = false)
        {
            if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
                return null;
            var interfaces = includeParents ? namedTypeSymbol.AllInterfaces : namedTypeSymbol.Interfaces;
            if (interfaces.Length <= 0)
                return null;
            foreach (var @interface in interfaces)
            {
                var fullName = @interface.ToDisplayString();
                if (fullName == interfaceName)
                    return (namedTypeSymbol, @interface);
            }

            return null;
        }

        public struct SimpleMethodParam
        {
            public string Name { get; set; }
            public string TypeName { get; set; }
        }

        public static string BuildParam(this SimpleMethodParam param)
        {
            return $"{param.TypeName} {param.Name}";
        }

        public static string BuildParams(this IEnumerable<SimpleMethodParam> @params)
        {
            return BuildSequentialValues(@params.Select(BuildParam), ",");
        }
        public static string BuildSequentialValues(this IEnumerable<string> values, string separator = ", ")
        {
            var paramsArray = values.ToArray();
            var length = paramsArray.Length;
            return length switch
            {
                0 => "",
                1 => paramsArray.First(),
                _ => string.Join(", ", paramsArray)
            };
        }
        public static string BuildArg(this SimpleMethodParam param)
        {
            return $"{param.Name}";
        }
        public static string BuildArgs(this IEnumerable<SimpleMethodParam> @params)
        {
            return BuildSequentialValues(@params.Select(BuildArg), ",");
        }
        public static string BuildGeneric(string selfType, string embedType)
        {
            return embedType switch
            {
                CSharpReservedNames.Void => selfType,
                _ => $"{selfType}<{embedType}>"
            };
        }
        public static string BuildCallerReturn(string callMethod, string chainMethod, string chainMethodArg,
            string returnType)
        {
            switch (returnType)
            {
                case CSharpReservedNames.Void:
                {
                    return $@"
{callMethod};
{chainMethod}({chainMethodArg});
";
                }
                default:
                    const string returnName = "returnValue";
                    return $@"
var {returnName} = {callMethod};
{chainMethod}({chainMethodArg}, {returnName});
";
            }
        }

        public static IMethodSymbol? GetMethodWithName(this INamedTypeSymbol namedTypeSymbol, 
            string name, bool @static = false)
        {
            foreach (var member in namedTypeSymbol.GetMembers())
            {
                if (member is not IMethodSymbol methodSymbol || methodSymbol.IsStatic != @static)
                    continue;
                if (methodSymbol.Name == name)
                    return methodSymbol;
            }

            return null;
        }

        // public static bool IsPartialTaskMethodContainsAttribute(this INamedTypeSymbol namedTypeSymbol,
        //     string attName,
        //     List<(IMethodSymbol, AttributeData)> methods)
        // {
        //     methods.Clear();
        //     foreach (var member in namedTypeSymbol.GetMembers())
        //     {
        //         if (member is IMethodSymbol
        //             {
        //                 IsStatic: false, PartialImplementationPart: null, ReturnType: { Name: "UniTask" or "Task" }
        //             } methodSymbol)
        //         {
        //             var attributes = methodSymbol.GetAttributes();
        //             if (attributes.Length <= 0)
        //                 continue;
        //             foreach (var attributeData in attributes)
        //             {
        //                 var attClass = attributeData.AttributeClass;
        //                 if (attClass is null)
        //                     continue;
        //                 if (attClass.ToDisplayString() == attName)
        //                 {
        //                     methods.Add((methodSymbol, attributeData));
        //                 }
        //             }
        //         }
        //     }
        //
        //     return methods.Count > 0;
        // }


        public static INamedTypeSymbol? InheritBaseType(this INamedTypeSymbol typeSymbol, string baseType)
        {
            var parent = typeSymbol.BaseType;
            while (parent is not null)
            {
                if (parent.ToDisplayString() == baseType)
                    return parent;
                parent = parent.BaseType;
            }

            return null;
        }

        public static (ISymbol symbol, AttributeData attributeData)? IsContainsAttribute(this ISymbol typeSymbol, 
            string attributeDisplayName)
        {
            var attributes = typeSymbol.GetAttributes();
            foreach (var attributeData in attributes)
            {
                if (attributeData.AttributeClass?.ToDisplayString() == attributeDisplayName)
                    return (typeSymbol, attributeData);
            }
            return null;
        }

        public static IFieldSymbol? GetEnumMember(this ITypeSymbol enumSymbol, int val)
        {
            var members = enumSymbol.GetMembers();
            foreach (var member in members)
            {
                if (member is IFieldSymbol { HasConstantValue: true } memberField && (int)memberField.ConstantValue == val)
                {
                    return memberField;
                }
            }

            return null;
        }

        public static IFieldSymbol? GetEnumMember(this TypedConstant fieldSymbol)
        {
            var modifierMember = fieldSymbol.Type?.GetEnumMember((int)fieldSymbol.Value);
            return modifierMember;
        }

        public static (INamedTypeSymbol self, INamedTypeSymbol @interface)? InheritInterfaceWithoutGeneric(
            this ISymbol typeSymbol,
            string interfaceName, bool includeParents = false)
        {
            if (typeSymbol is not INamedTypeSymbol { Interfaces: { Length: > 0 } } namedTypeSymbol)
                return null;
            var interfaces = includeParents ? namedTypeSymbol.AllInterfaces : namedTypeSymbol.Interfaces;
            foreach (var @interface in interfaces)
            {
                if (@interface.IsGenericType && @interface.ToDisplayString()
                        .Contains(interfaceName))
                {
                    return (namedTypeSymbol, @interface);
                }
            }

            return null;
        }


        public static bool InheritInterface(this ITypeSymbol symbol, string interfaceName, bool goDeep)
        {
            var interfaces = goDeep ? symbol.AllInterfaces : symbol.Interfaces;
            foreach (var @interface in interfaces)
            {
                if (@interface.ToDisplayString() == interfaceName)
                    return true;
            }

            return false;
        }

        private static string FormArguments(ImmutableArray<ITypeSymbol> arguments)
        {
            if (arguments.Length == 0)
                return "";
            if (arguments.Length == 1)
                return arguments[0].ToDisplayString();
            return string.Join(", ", arguments.Select(e => e.ToDisplayString()));
        }

        public static INamedTypeSymbol? InheritClass(this INamedTypeSymbol symbol, string baseName, int genericCount = 0)
        {
            if (symbol.ToDisplayString() == baseName)
                return symbol;
            var baseType = symbol.BaseType;
            while (baseType is not null)
            {
                if (genericCount == 0 || baseType.TypeParameters.Length == genericCount)
                {
                    var compareName = baseName;
                    if (genericCount > 0)
                        compareName = $"{baseName}<{FormArguments(baseType.TypeArguments)}>";
                    if (baseType.ToDisplayString() == compareName)
                        return baseType;
                }

                baseType = baseType.BaseType;
            }

            return null;
        }

        public static IEnumerable<(IFieldSymbol, AttributeData)> GetFieldsWithAttribute(this INamedTypeSymbol symbol, string attributeName)
        {
            foreach (var member in symbol.GetMembers())
            {
                if (member is not IFieldSymbol fieldSymbol)
                    continue;
                var att = IsContainsAttribute(fieldSymbol, attributeName);
                if (att is not null)
                    yield return (fieldSymbol, att.Value.attributeData);
            }
        }
    
        public static ITypeSymbol? GetMemberFieldType(ISymbol? member, bool? isReadonly = false)
        {
            ITypeSymbol? fieldType = null;
            if (member is IPropertySymbol { IsIndexer: false } propertySymbol && (isReadonly is null || propertySymbol.IsReadOnly == isReadonly))
                fieldType = propertySymbol.Type;
            else if (member is IFieldSymbol { AssociatedSymbol: null } fieldSymbol &&  (isReadonly is null || fieldSymbol.IsReadOnly == isReadonly))
                fieldType = fieldSymbol.Type;
            return fieldType;
        }
        public static INamedTypeSymbol? GetNamedTypeSymbol(this SyntaxNode typeDeclarationSyntax,
            Compilation compilation)
        {
            var semanticModel = compilation.GetSemanticModel(typeDeclarationSyntax.SyntaxTree);
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclarationSyntax);
            return typeSymbol as INamedTypeSymbol;
        }

        // public static INamedTypeSymbol? GetNamedTypeSymbol(this GeneratorSyntaxContext context)
        // {
        //     return context.SemanticModel.GetDeclaredSymbol(context.Node) as INamedTypeSymbol;
        // }
        //
        // public static INamedTypeSymbol? GetClassOrStructInheritInherit(this GeneratorSyntaxContext context,
        //     string interfaceName, bool goDeep)
        // {
        //     var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
        //     if (symbol is not INamedTypeSymbol namedTypeSymbol || !namedTypeSymbol.InheritInterface(interfaceName, goDeep))
        //     {
        //         return null;
        //     }
        //
        //     return namedTypeSymbol;
        // }

        public static uint GetBaseTypeSize(this INamedTypeSymbol memberType)
        {
            switch (memberType.SpecialType)
            {
                case SpecialType.System_Enum:
                    return 4;
                case SpecialType.System_Boolean:
                    return sizeof(bool);
                case SpecialType.System_Char:
                    return sizeof(char);
                case SpecialType.System_SByte:
                    return sizeof(sbyte);
                case SpecialType.System_Byte:
                    return sizeof(byte);
                case SpecialType.System_Int16:
                    return sizeof(short);
                case SpecialType.System_UInt16:
                    return sizeof(ushort);
                case SpecialType.System_Int32:
                    return sizeof(int);
                case SpecialType.System_UInt32:
                    return sizeof(uint);
                case SpecialType.System_Int64:
                    return sizeof(long);
                case SpecialType.System_UInt64:
                    return sizeof(ulong);
                case SpecialType.System_Decimal:
                    return sizeof(decimal);
                case SpecialType.System_Single:
                    return sizeof(float);
                case SpecialType.System_Double:
                    return sizeof(double);
            }

            return 0;
        }

        public static uint RecursiveSize(this INamedTypeSymbol namedTypeSymbol)
        {
            if (!namedTypeSymbol.IsUnmanagedType)
                return default;
            uint size = 0;
            foreach (var memberType in namedTypeSymbol.GetTypeMembers())
            {
                if (memberType.TypeKind == TypeKind.Struct)
                {
                    var embedSize = namedTypeSymbol.RecursiveSize();
                    if (embedSize == 0)
                        return 0;
                    size += embedSize;
                }
                else
                    size += memberType.GetBaseTypeSize();
            }

            return size;
        }
    }
}