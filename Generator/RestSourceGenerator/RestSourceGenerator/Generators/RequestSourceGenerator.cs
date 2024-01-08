using System.IO;
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
        private const string FileName = "summer-rest-generated.GeneratorTest.additionalfile";
        private const string AssemblyName = "SummerRest";
        public void Initialize(GeneratorInitializationContext context)
        {
        }
        public void Execute(GeneratorExecutionContext context)
        {
            var files = context.AdditionalFiles;
            var file = files.FirstOrDefault(e => Path.GetFileName(e.Path) == FileName);
            var text = file?.GetText();
            if (context.Compilation.AssemblyName != AssemblyName || file is null || text is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(nameof(SummerRestRequestsSourceGenerator), "No file", 
                        "Generated file named '{0}' does not exist", "Debug", DiagnosticSeverity.Warning, true), Location.None, 
                    FileName));
                return;
            }
            Request? request = JsonConvert.DeserializeObject<Request>(text.ToString());
            if (request is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(nameof(SummerRestRequestsSourceGenerator), "Wrong format", 
                        "The format of the generated file can not be deserialized as JSON object", "Debug", DiagnosticSeverity.Warning, true), Location.None));
                return;
            }
            var builder = new StringBuilder();
            request.Value.BuildClass(builder);
            context.GenerateFormattedCode("SummerRestRequests.g.cs", builder.ToString());
        }
    }
}