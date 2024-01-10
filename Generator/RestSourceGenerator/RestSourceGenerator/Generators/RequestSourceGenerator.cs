using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using RestSourceGenerator.Generators.Models;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Generators
{
    [Generator]
    public class SummerRestRequestsSourceGenerator : ISourceGenerator
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
                    new DiagnosticDescriptor(nameof(SummerRestRequestsSourceGenerator), "No file", 
                        "Generated file named '{0}' does not exist", "Debug", DiagnosticSeverity.Warning, true), Location.None, 
                    FileName));
                return;
            }
            Configuration? conf = JsonConvert.DeserializeObject<Configuration>(text.ToString());
            if (conf is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(nameof(SummerRestRequestsSourceGenerator), "Wrong format", 
                        "The format of the generated file can not be deserialized as JSON object", "Debug", DiagnosticSeverity.Warning, true), Location.None));
                return;
            }
            if (context.Compilation.AssemblyName != conf.Value.Assembly)
                return;
            var builder = new StringBuilder();
            builder.Append("namespace SummerRest.Requests {");
            foreach (var request in conf.Value.Domains)
                request.BuildClass(builder);
            builder.Append("}");
            context.GenerateFormattedCode("SummerRestRequests", builder.ToString());
        }
    }
}