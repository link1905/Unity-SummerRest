using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using SharedSourceGenerator.Utilities;

namespace RestSourceGenerator.Generators
{
    public enum DataFormat
    {
        Json, Xml, PlainText
    }
    public struct AuthContainer
    {
        public string AuthKey { get; set; }
        public string AppenderType { get; set; }
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
        public HttpMethod? Method { get; set; }
        public int? TimeoutSeconds { get; set; }
        public int? RedirectsLimit { get; set; }
        public ContentType? ContentType { get; set; }
        public KeyValue[]? Headers { get; set; }
        public KeyValue[]? RequestParams { get; set; }
        public AuthContainer? AuthContainer { get; set; }
        public DataFormat DataFormat { get; set; }
        public string? SerializedBody { get; set; }
        public IEnumerable<Request>? Services { get; set; }
        public IEnumerable<Request>? Requests { get; set; }

        private string BuildHeaders()
        {
            if (Headers is not { Length: > 0 })
                return string.Empty;
            return Headers.BuildSequentialValues(e => $@"Headers.Add(""{e.Key})"", ""{e.Value})""", ";");
        }

        private string BuildParams()
        {
            if (RequestParams is not { Length: > 0 })
                return string.Empty;
            return RequestParams.BuildSequentialValues(e => $@"Params.AddParam(""{e.Key})"", ""{e.Value})""", ";");
        }

        private string BuildAuth()
        {
            if (!AuthContainer.HasValue)
                return string.Empty;
            return $@"
AuthAppender = IAuthAppender<{AuthContainer.Value.AppenderType}>.GetSingleton();
AuthKey = {AuthContainer.Value.AuthKey};
";
        }

        private string BuildBody()
        {
            if (SerializedBody is null)
                return string.Empty;
            return $@"BodyData = DefaultDataSerializer.StaticDeserialize<TRequestBody>(@""{SerializedBody}"", DataFormat.Json)";
        }
        public void BuildRequest(StringBuilder builder)
        {
            var method = $"HttpMethod.{Method}";
            var timeout = TimeoutSeconds.HasValue ? $"{nameof(TimeoutSeconds)} = {TimeoutSeconds.Value};" : string.Empty;
            var redirects = RedirectsLimit.HasValue ? $"{nameof(RedirectsLimit)} = {RedirectsLimit.Value};" : string.Empty;
            var contentType = ContentType.HasValue ? 
                $@"{nameof(ContentType)} = new ContentType(""{ContentType.Value.MediaType}"", ""{ContentType.Value.CharSet}"", ""{ContentType.Value.Boundary}"")" : string.Empty;
            var headers = BuildHeaders();
            var @params = BuildParams();
            var auth = BuildAuth();
            var dataFormat = $"BodyFormat = DataFormat.{DataFormat}";
            var body = BuildBody();
            builder.Append($@"
public class {EndpointName} : BaseRequest<{EndpointName}>
{{
    public {EndpointName}() : base({Url}) 
    {{
        Method = {method};
        {timeout}
        {redirects}
        {contentType}
        {headers}
        {@params}
        {auth}
        Init();
    }}
}}
public class {EndpointName}<TRequestBody> : {EndpointName}, IWebRequest<TRequestBody>
{{
    public TRequestBody BodyData {{ get; set; }}
    public DataFormat BodyFormat {{ get; set; }}
    public override string SerializedBody => BodyData is null ? null : IDataSerializer.Current.Serialize(BodyData, BodyFormat);
    public {EndpointName}() : base({Url}) 
    {{
        {dataFormat}
        {body}
    }}
}}
");
        }
        public void BuildClass(StringBuilder builder)
        {
            if (TypeName == "Request")
            {
                BuildRequest(builder);
            }
            else
            {
                builder.Append($"public class {EndpointName} {{");
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
                    new DiagnosticDescriptor(nameof(RequestSourceGenerator), "No file", 
                        "Generated file named '{0}' does not exist", "Debug", DiagnosticSeverity.Warning, true), Location.None, 
                    FileName));
                return;
            }
            Request? request = JsonConvert.DeserializeObject<Request>(text.ToString());
            if (request is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(nameof(RequestSourceGenerator), "Wrong format", 
                        "The format of the generated file can not be deserialized as JSON object", "Debug", DiagnosticSeverity.Warning, true), Location.None));
                return;
            }
            var builder = new StringBuilder();
            request.Value.BuildClass(builder);
            context.GenerateFormattedCode("SummerRestRequests.g.cs", builder.ToString());
        }
    }
}