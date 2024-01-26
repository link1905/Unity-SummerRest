using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Generators
{
    [Generator]
    public class DefaultsGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<InterfaceDeclarationSyntax> Nodes { get; } = new();
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is not InterfaceDeclarationSyntax interfaceDeclarationSyntax)
                    return;
                if (interfaceDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword) && interfaceDeclarationSyntax.AttributeLists.Count > 0)
                    Nodes.Add(interfaceDeclarationSyntax);
            }
        }

        private List<(INamedTypeSymbol typeSymbol, AttributeData att)> GetGeneratedInterfaces(Compilation compilation, SyntaxReceiver syntaxReceiver)
        {
            var result = new List<(INamedTypeSymbol typeSymbol, AttributeData att)>();
            foreach (var syntaxNode in syntaxReceiver.Nodes)
            {
                var symbol = syntaxNode.GetNamedTypeSymbol(compilation);
                var att = symbol?.GetAttributeWithName("SummerRest.Runtime.Attributes.GeneratedDefaultAttribute");
                if (att is null)
                    continue;
                result.Add((symbol, att));
            }
            return result;
        }
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.Compilation.AssemblyName != "SummerRest")
                return;
            if (context.SyntaxReceiver is not SyntaxReceiver receiver)
                return;
            var interfaces = GetGeneratedInterfaces(context.Compilation, receiver);
            var conf = ConfigLoader.LoadJsonDocument(context);
            foreach (var (typeSymbol, att) in interfaces)
            {
                var typeName = typeSymbol.Name;
                var @namespace = typeSymbol.ContainingNamespace.ToDisplayString();
                var propName = att.ConstructorArguments[0].Value as string;
                var @default = (att.ConstructorArguments[1].Value as INamedTypeSymbol)?.ToDisplayString();
                string? genType;
                if (conf is not null && conf.RootElement.TryGetProperty(propName, out var prop))
                    genType = prop.GetString();
                else
                    genType = @default;
                if (genType is null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor(nameof(RestSourceGenerator), "Unidentified default support interface", 
                            "Could not detect the default type of the interface {0}", "Debug", DiagnosticSeverity.Warning, true), Location.None, typeName));
                    continue;
                }
                context.GenerateFormattedCode(typeName, $@"
using SummerRest.Runtime.DataStructures;
namespace {@namespace}
{{
    public partial interface {typeName} : IDefaultSupport<{typeName}, {genType}>
    {{
    }}
}}
");
            }
        }
    }
}