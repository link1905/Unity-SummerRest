using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using RestSourceGenerator.Generators.Models;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Generators
{
    [Generator]
    public class RestSourceGenerator : ISourceGenerator
    {
        private const string FileName = "summer-rest-generated.SummerRestRequestsGenerator.additionalfile";
        // private const string AssemblyName = "SummerRest";
        public void Initialize(GeneratorInitializationContext context)
        {
        }
        public void Execute(GeneratorExecutionContext context)
        {
            var files = context.AdditionalFiles;
            var file = files.FirstOrDefault(e => e.Path.Contains(FileName));
            var text = file?.GetText();
            if (file is null || text is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(nameof(RestSourceGenerator), "No file", 
                        "Generated file named '{0}' does not exist", "Debug", DiagnosticSeverity.Warning, true), Location.None, 
                    FileName));
                return;
            }
            Configuration? conf = JsonConvert.DeserializeObject<Configuration>(text.ToString());
            if (conf is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(nameof(RestSourceGenerator), "Wrong format", 
                        "The format of the generated file can not be deserialized as JSON object", "Debug", DiagnosticSeverity.Warning, true), Location.None));
                return;
            }
            
            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(nameof(RestSourceGenerator), "Start generating", 
                    "Start generating source", "Debug", DiagnosticSeverity.Info, true), Location.None));
            if (context.Compilation.AssemblyName != conf.Value.Assembly)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(nameof(RestSourceGenerator), "Not target assembly", 
                        "'{0}' is not the target assembly for generating source", "Debug", DiagnosticSeverity.Info, true), 
                    Location.None, context.Compilation.AssemblyName));
                return;
            }
            var builder = new StringBuilder();
            builder.Append("namespace SummerRest.Requests {");
            foreach (var request in conf.Value.Domains)
                request.BuildClass(builder);
            builder.Append("}");
            context.GenerateFormattedCode("SummerRestRequests", builder.ToString());
            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(nameof(RestSourceGenerator), "Finish generating", 
                    "Finish generating source", "Debug", DiagnosticSeverity.Info, true), Location.None));
        }
    }
}