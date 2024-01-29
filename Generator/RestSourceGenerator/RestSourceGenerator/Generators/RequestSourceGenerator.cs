using System.Text;
using Microsoft.CodeAnalysis;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Generators
{
    [Generator]
    public class RestSourceGenerator : ISourceGenerator
    {
        // private const string AssemblyName = "SummerRest";
        public void Initialize(GeneratorInitializationContext context)
        {
        }
        public void Execute(GeneratorExecutionContext context)
        {
            var conf = ConfigLoader.Load(context);
            if (conf is null)
                return;
            if (context.Compilation.AssemblyName != conf.Value.Assembly)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(nameof(RestSourceGenerator), "Not target assembly", 
                        "'{0}' is not the target assembly for generating source", "Debug", DiagnosticSeverity.Info, true), 
                    Location.None, context.Compilation.AssemblyName));
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(nameof(RestSourceGenerator), "Start generating", 
                    "Start generating source", "Debug", DiagnosticSeverity.Info, true), Location.None));
            var builder = new StringBuilder();
            builder.Append(@"
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Parsers;
using UnityEngine.Networking;
namespace SummerRest.Runtime.Requests {");
            if (conf.Value.Domains is not null)
            {
                foreach (var request in conf.Value.Domains)
                    request.BuildClass(builder);
            }

            builder.Append("}");
            context.GenerateFormattedCode("SummerRestRequests", builder.ToString());
            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(nameof(RestSourceGenerator), "Finish generating", 
                    "Finish generating source", "Debug", DiagnosticSeverity.Info, true), Location.None));
        }
    }
}