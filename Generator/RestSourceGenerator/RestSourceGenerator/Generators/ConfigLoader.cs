using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using RestSourceGenerator.Metadata;

namespace RestSourceGenerator.Generators
{
    public static class ConfigLoader
    {
        private const string FileName = "summer-rest-generated.RestSourceGenerator.additionalfile";
        
        private static readonly DiagnosticDescriptor WrongFormat = new(nameof(RestSourceGenerator), "Wrong format", 
            "The format of the generated file can not be deserialized as XML object", "Debug", DiagnosticSeverity.Warning, true);

        private static readonly DiagnosticDescriptor AbsentFile = new(
            nameof(RestSourceGenerator), "No file",
            "Generated file named does not exist", "Debug", DiagnosticSeverity.Warning, true);
        public static Configuration? Load(GeneratorExecutionContext context)
        {
            var text = LoadRaw(context);
            if (text is null)
                return null;
            var serializer = new XmlSerializer(typeof(Configuration));
            using var reader = new StringReader(text);
            var result = serializer.Deserialize(reader);
            if (result is not null) 
                return (Configuration)result;
            context.ReportDiagnostic(Diagnostic.Create(WrongFormat, Location.None));
            return null;
        } 
        
        public static XmlDocument? LoadJsonDocument(GeneratorExecutionContext context)
        {
            var text = LoadRaw(context);
            if (text is null)
                return null;
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(text);
                return xmlDocument;
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(WrongFormat, Location.None));
                return null;
            }
        } 
        
        private static string? LoadRaw(GeneratorExecutionContext context)
        {
            var files = context.AdditionalFiles;
            var file = files.FirstOrDefault(e => e.Path.Contains(FileName));
            var text = file?.GetText();
            if (file is null || text is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(AbsentFile
                    , Location.None));
                return null;
            }
            return text.ToString();
        } 
    }
}