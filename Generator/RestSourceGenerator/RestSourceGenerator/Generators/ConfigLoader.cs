using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using RestSourceGenerator.Metadata;

namespace RestSourceGenerator.Generators
{
    public static class ConfigLoader
    {
        private const string FileName = "summer-rest-generated.RestSourceGenerator.additionalfile";
        public static Configuration? Load(GeneratorExecutionContext context)
        {
            var text = LoadRaw(context);
            if (text is null)
                return null;
            Configuration? conf = System.Text.Json.JsonSerializer.Deserialize<Configuration>(text);
            if (conf is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(nameof(RestSourceGenerator), "Wrong format", 
                        "The format of the generated file can not be deserialized as JSON object", "Debug", DiagnosticSeverity.Warning, true), Location.None));
                return null;
            }
            return conf;
        } 
        
        public static JsonDocument? LoadJsonDocument(GeneratorExecutionContext context)
        {
            var text = LoadRaw(context);
            if (text is null)
                return null;
            var conf = JsonDocument.Parse(text);
            return conf;
        } 
        
        private static string? LoadRaw(GeneratorExecutionContext context)
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
                return null;
            }
            return text.ToString();
        } 
    }
}