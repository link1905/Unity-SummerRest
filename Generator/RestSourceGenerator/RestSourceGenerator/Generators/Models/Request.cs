using System.Collections.Generic;
using System.Text;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Generators.Models
{
    public struct Request
    {
        public string TypeName { get; set; }
        public string EndpointName { get; set; }
        public string? Url { get; set; }
        public string? UrlWithParams { get; set; }
        public HttpMethod? Method { get; set; }
        public int? TimeoutSeconds { get; set; }
        public int? RedirectsLimit { get; set; }
        public ContentType? ContentType { get; set; }
        public KeyValue[]? Headers { get; set; }
        public KeyValue[]? RequestParams { get; set; }
        public AuthContainer? AuthContainer { get; set; }
        public DataFormat? DataFormat { get; set; }
        public string? SerializedBody { get; set; }
        public IEnumerable<Request>? Services { get; set; }
        public IEnumerable<Request>? Requests { get; set; }

        private string BuildHeaders()
        {
            if (Headers is not { Length: > 0 })
                return string.Empty;
            return Headers.BuildSequentialValues(e => $@"Headers.Add(""{e.Key}"", ""{e.Value}"")", ";") + ";";
        }

        private string BuildParams()
        {
            if (RequestParams is not { Length: > 0 })
                return string.Empty;
            return RequestParams.BuildSequentialValues(e => $@"Params.AddParam(""{e.Key}"", ""{e.Value}"")", ";") + ";";
        }

        private string BuildAuth()
        {
            if (!AuthContainer.HasValue)
                return string.Empty;
            return $@"
AuthAppender = IAuthAppender<{AuthContainer.Value.AppenderType}>.GetSingleton();
AuthKey = ""{AuthContainer.Value.AuthKey}"";
";
        }

        private string BuildBody()
        {
            if (string.IsNullOrEmpty(SerializedBody))
                return string.Empty;
            return $@"BodyData = DefaultDataSerializer.StaticDeserialize<TRequestBody>(@""{SerializedBody.Replace("\"", "\"\"")}"", DataFormat.{DataFormat});";
        }
        public void BuildRequest(StringBuilder builder)
        {
            var className = EndpointName.ToClassName();
            var method = $"HttpMethod.{Method}";
            var timeout = TimeoutSeconds.HasValue ? $"{nameof(TimeoutSeconds)} = {TimeoutSeconds.Value};" : string.Empty;
            var redirects = RedirectsLimit.HasValue ? $"{nameof(RedirectsLimit)} = {RedirectsLimit.Value};" : string.Empty;
            var contentType = ContentType.HasValue ? 
                $@"{nameof(ContentType)} = new ContentType(""{ContentType.Value.MediaType}"", ""{ContentType.Value.CharSet}"", ""{ContentType.Value.Boundary}"");" : string.Empty;
            var headers = BuildHeaders();
            var @params = BuildParams();
            var auth = BuildAuth();
            var dataFormat = $"BodyFormat = DataFormat.{DataFormat};";
            var body = BuildBody();
            builder.Append($@"
public class {className} : BaseRequest<{className}>
{{
    public {className}() : base(""{Url}"", ""{UrlWithParams}"") 
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
public class {className}<TRequestBody> : {className}, IWebRequest<TRequestBody>
{{
    public TRequestBody BodyData {{ get; set; }}
    public DataFormat BodyFormat {{ get; set; }}
    public override string SerializedBody => BodyData is null ? null : IDataSerializer.Current.Serialize(BodyData, BodyFormat);
    public {className}() : base(""{Url}"", ""{UrlWithParams}"") 
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
                builder.Append($"public class {EndpointName.ToClassName()} {{");
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
                builder.Append("}");
            }
        }
    }
}