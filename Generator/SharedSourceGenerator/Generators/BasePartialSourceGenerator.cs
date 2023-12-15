using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharedSourceGenerator.Metadata;
using SharedSourceGenerator.Utilities;

namespace SharedSourceGenerator.Generators
{
    public abstract class BasePartialSourceGenerator<TExecuteSymbol> : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new BaseSyntaxReceiver());
        }
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not BaseSyntaxReceiver
                {
                    TypeToAugments: { Count: > 0 }
                } receiver) return;
            foreach (var type in receiver.TypeToAugments)
            {
                var symbol = GetSemanticTargetForGeneration(type, context.Compilation);
                if (symbol is not null)
                {
                    Execute(context, symbol);
                }
            }
        }
        private class BaseSyntaxReceiver : ISyntaxReceiver
        {
            public readonly List<TypeDeclarationSyntax> TypeToAugments = new();
            public void OnVisitSyntaxNode(SyntaxNode node)
            {
                if (!node.IsPartialNode())
                    return;
                if (node is not TypeDeclarationSyntax typeDec)
                    return;
                TypeToAugments.Add(typeDec);
            }
        }
        protected abstract TExecuteSymbol? GetSemanticTargetForGeneration(SyntaxNode typeDeclarationSyntax,
            Compilation compilation);
        protected abstract void Execute(GeneratorExecutionContext context, TExecuteSymbol target);
    }
}