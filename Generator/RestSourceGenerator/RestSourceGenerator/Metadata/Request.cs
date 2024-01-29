using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Metadata
{
    public class Request
    {
        [XmlAttribute]
        public string? TypeName { get; set; }
        [XmlAttribute]
        public string? EndpointName { get; set; }
        [XmlAttribute]
        public string? Url { get; set; }
        [XmlAttribute]
        public string? UrlWithParams { get; set; }
        [XmlAttribute]
        public HttpMethod Method { get; set; }
        [XmlAttribute] public int TimeoutSeconds { get; set; } = -1;
        [XmlAttribute] public int RedirectsLimit { get; set; } = -1;
        [XmlAttribute]
        public DataFormat DataFormat { get; set; }
        [XmlAttribute]
        public string? SerializedBody { get; set; }
        [XmlAttribute]
        public bool IsMultipart { get; set; }
        [XmlElement]
        public ContentType? ContentType { get; set; }
        [XmlArray]
        public KeyValue[]? Headers { get; set; }
        [XmlArray]
        public KeyValue[]? RequestParams { get; set; }
        [XmlElement]
        public AuthContainer? AuthContainer { get; set; }

        [XmlArray]
        public KeyValue[]? SerializedForm { get; set; }
        [XmlArray]
        [XmlArrayItem("Service")]
        public Request[]? Services { get; set; }
        [XmlArray]
        [XmlArrayItem("Request")]
        public Request[]? Requests { get; set; }

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
            return RequestParams.BuildSequentialValues(e => $@"Params.AddParamToList(""{e.Key}"", ""{e.Value}"")", ";") + ";";
        }

        private string BuildBaseClass()
        {
            if (!IsMultipart)
                return $"SummerRest.Runtime.Requests.BaseDataRequest<{EndpointName.ToClassName()}>";
            return $"SummerRest.Runtime.Requests.BaseMultipartRequest<{EndpointName.ToClassName()}>";
            //return $"SummerRest.Runtime.Requests.BaseAuthRequest<{EndpointName.ToClassName()}, {AuthContainer.Value.AppenderType}, {AuthContainer.Value.AuthDataType}>";
        }
        private (string authClass, string authKey) BuildAuth()
        {
            if (!AuthContainer.HasValue)
                return ("null", string.Empty);
            return ($@"
IRequestModifier<AuthRequestModifier<{AuthContainer.Value.AppenderType}, {AuthContainer.Value.AuthDataType}>>.GetSingleton()", 
                $@"
AuthKey = ""{AuthContainer.Value.AuthKey}"";
");
        }

        private string BuildBody()
        {
            if (!IsMultipart)
            {
                if (string.IsNullOrEmpty(SerializedBody))
                    return $"BodyFormat = DataFormat.{DataFormat};";
                return $@"
BodyFormat = DataFormat.{DataFormat};
InitializedSerializedBody = @""{SerializedBody.Replace("\"", "\"\"")}"";";
            }
            if (SerializedForm is not {Length: >0})
                return string.Empty;
            return SerializedForm.BuildSequentialValues(e => $@"MultipartFormSections.Add(new MultipartFormDataSection(""{e.Key}"", ""{e.Value}""))", ";") + ";";
        }
        public void BuildRequest(StringBuilder builder)
        {
            var className = EndpointName.ToClassName();
            var method = $"HttpMethod.{Method}";
            var timeout = TimeoutSeconds >= 0 ? $"{nameof(TimeoutSeconds)} = {TimeoutSeconds};" : string.Empty;
            var redirects = RedirectsLimit >= 0 ? $"{nameof(RedirectsLimit)} = {RedirectsLimit};" : string.Empty;
            var contentType = ContentType.HasValue ? 
                $@"{nameof(ContentType)} = new ContentType(""{ContentType.Value.MediaType}"", ""{ContentType.Value.Charset}"", ""{ContentType.Value.Boundary}"");" : string.Empty;
            var headers = BuildHeaders();
            var @params = BuildParams();
            var (authProp, authKey) = BuildAuth();
            var body = BuildBody();
            builder.Append($@"
public class {className} : {BuildBaseClass()}
{{
    public {className}() : base(""{Url}"", ""{UrlWithParams}"", {authProp}) 
    {{
        Method = {method};
        {timeout}
        {redirects}
        {contentType}
        {headers}
        {@params}
        {authKey}
        {body}
        Init();
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
                builder.Append($"public static class {EndpointName.ToClassName()} {{");
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