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

            var authKeys = conf.Value.BuildAuthClass();
            
            var requestBuilder = new StringBuilder();
            requestBuilder.Append($@"
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Parsers;
using UnityEngine.Networking;
using System.Collections.Generic;
namespace SummerRest.Runtime.Authenticate
{{
    public static class AuthKeys
    {{
        {authKeys}
    }}
}}

namespace SummerRest.Runtime.Requests {{");
            conf.Value.BuildDomainClasses(requestBuilder);
            requestBuilder.Append("}");
            
            context.GenerateFormattedCode("SummerRestRequests", requestBuilder.ToString());
            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(nameof(RestSourceGenerator), "Finish generating", 
                    "Finish generating source", "Debug", DiagnosticSeverity.Info, true), Location.None));
        }
    }
}