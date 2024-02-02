using System;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Metadata
{
    public class Request
    {
        [XmlAttribute]
        public string TypeName { get; set; }
        [XmlAttribute]
        public string EndpointName { get; set; }
        [XmlAttribute]
        public string? Url { get; set; }
        [XmlAttribute]
        public string? UrlFormat { get; set; }
        [XmlAttribute]
        public string? UrlWithParams { get; set; }
        [XmlArray]
        public KeyValue[]? UrlFormatContainers { get; set; }
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
        private (string format, string keys, string values) BuildUrlFormatKeys()
        {
            if (UrlFormat is null || UrlFormatContainers is not { Length: > 0 })
                return ("null", string.Empty, "System.Array.Empty<string>()");
            var keys = UrlFormatContainers.Select(e => e.Key).BuildSequentialValues(
                (key, idx) => $"public const int {key.ToClassName()} = {idx}", ";") + ";";
            var valueElements = UrlFormatContainers.Select(e => e.Value).BuildSequentialValues((v, _) => $@"""{v}""");
            var values = $"new string[] {{{valueElements}}}";
            return ($@"""{UrlFormat}""", keys, values);
        }

        private (string, string) BuildKeysAndOriginalValues(KeyValue[]? keyValues, string containerName, Func<KeyValue, string> buildAdder)
        {
            if (keyValues is not { Length: > 0 })
                return (string.Empty, string.Empty);
            // return RequestParams.BuildSequentialValues((e, _) => $@"Params.AddParamToList(""{e.Key}"", ""{e.Value}"")", ";") + ";";
            var keys =  keyValues.BuildSequentialValues((key, _) => $@"public const string {key.Key.ToClassName()} = ""{key.Key}""", ";") + ";";
            var values = keyValues.BuildSequentialValues((e, _) =>
            {
                var keyProp = $"Keys.{containerName}.{e.Key.ToClassName()}";
                return buildAdder(new KeyValue
                {
                    Key = keyProp,
                    Value = e.Value
                }); // 
            }, ";") + ";";
            return (keys, values);
        }
        
        private (string keys, string headers) BuildHeaders()
        {
            return BuildKeysAndOriginalValues(Headers, "Headers", kv => $@"Headers.TryAdd({kv.Key}, ""{kv.Value}"")");
        }
        private (string keys, string @params) BuildParams()
        {
            return BuildKeysAndOriginalValues(RequestParams, "Params", kv => $@"Params.AddParamToList({kv.Key}, ""{kv.Value}"")");
        }
        private string BuildBaseClass()
        {
            if (!IsMultipart)
                return $"SummerRest.Runtime.Requests.BaseDataRequest<{EndpointName.ToClassName()}>";
            return $"SummerRest.Runtime.Requests.BaseMultipartRequest<{EndpointName.ToClassName()}>";
        }
        private (string authClass, string authKey) BuildAuth()
        {
            if (!AuthContainer.HasValue)
                return ("null", string.Empty);
            return ($@"
IRequestModifier<AuthRequestModifier<{AuthContainer.Value.AppenderType}, {AuthContainer.Value.AuthDataType}>>.GetSingleton()", 
                $@"
AuthKey = SummerRest.Runtime.Authenticate.Repositories.AuthKeys.{AuthContainer.Value.AuthKey.ToClassName()};
");
        }

        private (string keys, string body) BuildBody()
        {
            if (!IsMultipart)
            {
                if (string.IsNullOrEmpty(SerializedBody))
                    return (string.Empty, $"BodyFormat = DataFormat.{DataFormat};");
                return (string.Empty, $@"
BodyFormat = DataFormat.{DataFormat};
InitializedSerializedBody = @""{SerializedBody.Replace("\"", "\"\"")}"";");
            }
            if (SerializedForm is not {Length: >0})
                return (string.Empty, string.Empty);
            return BuildKeysAndOriginalValues(SerializedForm, "MultipartFormSections", kv =>
            {
                if (kv.Value is null)
                    return null;
                return $@"MultipartFormSections.Add(new MultipartFormDataSection({kv.Key}, ""{kv.Value}""))";
            });
        }
        public void BuildRequest(StringBuilder builder)
        {
            var className = EndpointName.ToClassName();
            var method = $"HttpMethod.{Method}";
            var timeout = TimeoutSeconds >= 0 ? $"{nameof(TimeoutSeconds)} = {TimeoutSeconds};" : string.Empty;
            var redirects = RedirectsLimit >= 0 ? $"{nameof(RedirectsLimit)} = {RedirectsLimit};" : string.Empty;
            var contentType = ContentType.HasValue ? 
                $@"{nameof(ContentType)} = new ContentType(""{ContentType.Value.MediaType}"", ""{ContentType.Value.Charset}"", ""{ContentType.Value.Boundary}"");" : string.Empty;
            var (urlFormat, urlFormatKeys, urlFormatValues) = BuildUrlFormatKeys();
            var (headerKeys, headers) = BuildHeaders();
            var (paramsKeys, @params) = BuildParams();
            var (authProp, authKey) = BuildAuth();
            var (multipartFormKeys, body) = BuildBody();
            builder.Append($@"
public sealed class {className} : {BuildBaseClass()}
{{
    public static class Keys 
    {{ 
        public static class UrlFormat {{
            {urlFormatKeys}
        }}
        public static class Headers {{
            {headerKeys}
        }}
        public static class Params {{
            {paramsKeys}
        }}
        public static class MultipartFormSections {{
            {multipartFormKeys}
        }}
    }}
    public {className}() : base(""{Url}"", ""{UrlWithParams}"", {urlFormat}, {urlFormatValues}, {authProp}) 
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