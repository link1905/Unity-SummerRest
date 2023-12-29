using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace RestSourceGenerator.Generators
{
    public enum DataFormat
    {
        Json, Xml, PlainText
    }

    public struct KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public enum HttpMethod
    {
        Get = 0, Post = 1, Put = 2, Delete = 3, Patch = 4, Head = 5, Options = 6, Trace = 7, Connect = 8
    }
    public struct ContentType
    {
        public string CharSet { get; set; }
        public string MediaType { get; set; }
        public string Boundary { get; set; }
    }
    public struct Request
    {
        public string TypeName { get; set; }
        public string Url { get; set; }
        public string EndpointName { get; set; }
        public KeyValue[] Headers { get; set; }
        public KeyValue[] RequestParams { get; set; }
        public DataFormat DataFormat { get; set; }
        public int TimeoutSeconds { get; set; }
        public int RedirectsLimit { get; set; }
        public ContentType ContentType { get; set; }
        public HttpMethod Method { get; set; }
        public IEnumerable<Request>? Services { get; set; }
        public IEnumerable<Request>? Requests { get; set; }
        public void BuildRequest(StringBuilder builder)
        {
            builder.Append(@"

var request = new UnityWebRequest();
request.
")
        }
        public void BuildClass(StringBuilder builder)
        {
            builder.Append($"public class {EndpointName} {{");
            if (TypeName == "Request")
            {
                BuildRequest(builder);
            }
            else
            {
                if (Requests is not null)
                {
                    foreach (var request in Requests)
                        request.BuildClass(builder);
                }
                if (Services is not null)
                {
                    foreach (var service in Services)
                        service.BuildClass(builder);
                }   
            }
            builder.Append("}");
        }
    } 
    public class RequestSourceGenerator : ISourceGenerator
    {
        private const string UnityTempPath = "Temp/SummerRestSourceGenIndicator.json";
        public void Initialize(GeneratorInitializationContext context)
        {
        }
        public void Execute(GeneratorExecutionContext context)
        {
            string? jsonContent = null;
            if (File.Exists(UnityTempPath))
            {
                jsonContent = File.ReadAllText(UnityTempPath);
            }
            else if (context.AdditionalFiles.Length > 0)
            {
                jsonContent = context.AdditionalFiles.FirstOrDefault().GetText().ToString();
            }
            if (jsonContent is null)
                return;
            Request? content = JsonConvert.DeserializeObject<Request>(jsonContent);
            if (content is null)
                return;
            var builder = new StringBuilder();
            foreach (var VARIABLE in content.Value.)
            {
                
            }
        }
    }
}