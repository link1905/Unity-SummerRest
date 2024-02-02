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

            var authKeys = conf.Value.AuthKeys is not { Length: > 0 }
                ? string.Empty
                : conf.Value.AuthKeys.BuildSequentialValues((k, _) => @$"public const string {k.ToClassName()} = ""{k}""", separator: ";") + ";";
            var requestBuilder = new StringBuilder();
            requestBuilder.Append($@"
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Parsers;
using UnityEngine.Networking;
namespace SummerRest.Runtime.Authenticate.Repositories
{{
    public static partial class AuthKeys
    {{
        {authKeys}
    }}
}}

namespace SummerRest.Runtime.Requests {{");
            if (conf.Value.Domains is not null)
            {
                foreach (var request in conf.Value.Domains)
                    request.BuildClass(requestBuilder);
            }

            requestBuilder.Append("}");
            
            
            context.GenerateFormattedCode("SummerRestRequests", requestBuilder.ToString());
            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(nameof(RestSourceGenerator), "Finish generating", 
                    "Finish generating source", "Debug", DiagnosticSeverity.Info, true), Location.None));
        }
    }
}