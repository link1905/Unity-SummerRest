using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Metadata
{
    public class Request
    {
        [XmlAttribute] public string TypeName { get; set; }
        [XmlAttribute] public string EndpointName { get; set; }

        public string EndpointClassName => EndpointName.ToClassName();
        [XmlAttribute] public string? Url { get; set; }
        [XmlAttribute] public string? UrlFormat { get; set; }
        [XmlAttribute] public string? UrlWithParams { get; set; }
        [XmlArray] public KeyValue[]? UrlFormatContainers { get; set; }
        [XmlAttribute] public HttpMethod Method { get; set; }
        [XmlAttribute] public int TimeoutSeconds { get; set; } = -1;
        [XmlAttribute] public int RedirectsLimit { get; set; } = -1;
        [XmlAttribute] public DataFormat DataFormat { get; set; }
        [XmlAttribute] public string? SerializedBody { get; set; }
        [XmlAttribute] public bool IsMultipart { get; set; }
        [XmlElement] public ContentType? ContentType { get; set; }
        [XmlArray] public KeyValue[]? Headers { get; set; }
        [XmlArray] public KeyValue[]? RequestParams { get; set; }
        [XmlElement] public AuthContainer? AuthContainer { get; set; }

        [XmlArray] public KeyValue[]? SerializedForm { get; set; }

        [XmlArray]
        [XmlArrayItem(nameof(ProjectReflection.SummerRestConfiguration.Service))]
        public Request[]? Services { get; set; }

        [XmlArray]
        [XmlArrayItem(nameof(ProjectReflection.SummerRestConfiguration.Request))]
        public Request[]? Requests { get; set; }

        public (string format, string keys, string values, string valuesArr) BuildUrlFormat()
        {
            if (UrlFormat is null || UrlFormatContainers is not { Length: > 0 })
                return (RoslynDefaultValues.Null, string.Empty, string.Empty,
                    RoslynDefaultValues.EmptyArray(RoslynDefaultValues.String));
            var keys = UrlFormatContainers
                .Select((e, i) => (e.Key, i.ToString()))
                .BuildSequentialConstants(RoslynDefaultValues.Int);
            var values = UrlFormatContainers
                .Select(e => e.DeconstructEmbedded())
                .BuildSequentialConstants(RoslynDefaultValues.String);
            var valuesArr = UrlFormatContainers
                .Select(e => $"{ProjectReflection.SummerRestConfiguration.Request.Values}.UrlFormat.{e.Key.ToClassName()}")
                .BuildArray(RoslynDefaultValues.String);
            return (UrlFormat.ToEmbeddedString(), keys, values, valuesArr);
        }

        public static (string keys, string values, string refValues) BuildKeysValuesRefValues(KeyValue[]? keyValues,
            string containerName, Func<KeyValue, string> buildAdder)
        {
            if (keyValues is not { Length: > 0 })
                return (string.Empty, string.Empty, string.Empty);
            var keys = keyValues
                .Select(e => e.Key)
                .ToImmutableSortedSet()
                .Select(e => (e, e.ToEmbeddedString()))
                .BuildSequentialConstants(RoslynDefaultValues.String);

            // 1 key can have multiple values 
            var keyToValues = keyValues
                .Where(e => e.Value is not null)
                .OrderBy(e => e.Key)
                .ToLookup(p => p.Key);
            var singleValues = keyToValues
                .Where(e => e.Count() == 1)
                .Select(e => (e.Key, e.First().Value.ToEmbeddedString()))
                .BuildSequentialConstants(RoslynDefaultValues.String);
            var multipleValues = keyToValues.
                Where(e => e.Count() >= 2)
                .Select(e => 
                    (e.Key, e.Select(keyValue => keyValue.Value.ToEmbeddedString()).BuildArray(RoslynDefaultValues.String)))
                .BuildSequentialConstants(RoslynDefaultValues.Array(RoslynDefaultValues.String));
            var refValues = keyToValues
                .BuildSequentialValues((e, _) => {
                var keyProp =
                    $"{ProjectReflection.SummerRestConfiguration.Request.Keys}.{containerName}.{e.Key.ToClassName()}";
                var valueProp =
                    $"{ProjectReflection.SummerRestConfiguration.Request.Values}.{containerName}.{e.Key.ToClassName()}";
                return buildAdder(new KeyValue
                {
                    Key = keyProp,
                    Value = valueProp
                });
            }, RoslynDefaultValues.SemiColons, RoslynDefaultValues.SemiColons);
            return (keys, string.Concat(singleValues, multipleValues), refValues);
        }

        private (string keys, string values, string headers) BuildHeaders()
        {
            return BuildKeysValuesRefValues(Headers, ProjectReflection.SummerRestConfiguration.Request.Headers,
                kv =>
                    $@"{ProjectReflection.SummerRestConfiguration.Request.Headers}.TryAdd({kv.Key}, {kv.Value})");
        }

        private (string keys, string values, string @params) BuildParams()
        {
            return BuildKeysValuesRefValues(RequestParams,
                ProjectReflection.SummerRestConfiguration.Request.Params,
                kv =>
                    $@"{ProjectReflection.SummerRestConfiguration.Request.Params}.AddParamToList({kv.Key}, {kv.Value})");
        }

        private string BuildBaseClass()
        {
            if (!IsMultipart)
                return $"{ProjectReflection.SummerRest.Runtime.Requests.BaseDataRequest}<{EndpointClassName}>";
            return $"{ProjectReflection.SummerRest.Runtime.Requests.BaseMultipartRequest}<{EndpointClassName}>";
        }

        public static (string authProp, string authKey) BuildAuth(AuthContainer? authContainer)
        {
            if (!authContainer.HasValue)
                return (RoslynDefaultValues.Null, string.Empty);
            return (
                $@"IRequestModifier<AuthRequestModifier<{authContainer.Value.AppenderType}, {authContainer.Value.AuthDataType}>>.GetSingleton()",
                $@"AuthKey = {ProjectReflection.SummerRest.Runtime.Authenticate.AuthKeys}.{authContainer.Value.AuthKey.ToClassName()};");
        }

        public static (string serializedValue, string body) BuildDataBody(string serializedBody, DataFormat dataFormat)
        {
            if (serializedBody is null)
                return (RoslynDefaultValues.Null, $"BodyFormat = DataFormat.{dataFormat};");
            var embeddedBody = $@"@{serializedBody.Replace("\"", "\"\"").ToEmbeddedString()}";
            return (embeddedBody, 
                $@"BodyFormat = DataFormat.{dataFormat};
InitializedSerializedBody = {ProjectReflection.SummerRestConfiguration.Request.Values}.SerializedBody;");
        }

        public static (string keys, string values, string body) BuildMultipartBody(KeyValue[]? serializedForm)
        {
            if (serializedForm is not { Length: > 0 })
                return (string.Empty, string.Empty, string.Empty);
            return BuildKeysValuesRefValues(serializedForm,
                ProjectReflection.SummerRestConfiguration.Request.MultipartFormSections,
                kv => $@"{ProjectReflection.SummerRestConfiguration.Request.MultipartFormSections}.Add(new MultipartFormDataSection({kv.Key}, {kv.Value}))");
        }

        public static (string constValue, string contentType) BuildContentType(ContentType? contentType)
        {
            if (!contentType.HasValue)
                return (RoslynDefaultValues.Null, string.Empty);
            var contentTypeCreator = contentType.Value.ToInstance();
            return (contentTypeCreator, $"{nameof(ContentType)} = {ProjectReflection.SummerRestConfiguration.Request.Values}.ContentType;");
        }

        public static string BuildIntField(int value, string fieldName)
        {
            return value >= 0 ? $"{fieldName} = {value};" : string.Empty;
        }

        public void BuildRequest(StringBuilder builder)
        {
            var className = EndpointName.ToClassName();
            var method = $"HttpMethod.{Method}";
            var timeout = BuildIntField(TimeoutSeconds, nameof(TimeoutSeconds));
            var redirects = BuildIntField(RedirectsLimit, nameof(RedirectsLimit));
            var (contentTypeConstValue, contentType) = BuildContentType(ContentType);
            var (urlFormat, urlFormatKeys, urlFormatValues, urlFormatValuesArr) 
                = BuildUrlFormat();
            var (headerKeys, headerValues, headers) = BuildHeaders();
            var (paramsKeys, paramsValues, @params) = BuildParams();
            var (authProp, authKey) = BuildAuth(AuthContainer);

            var (multipartFormKeys, multipartFormValues, multipartBody) = BuildMultipartBody(SerializedForm);
            var (serializedValue, dataBody) = BuildDataBody(SerializedBody, DataFormat);
            var body = IsMultipart ? multipartBody : dataBody;
            // Instead of directly embed the keys and values into the class fields
            // We create regarding static fields to decrease string allocations
            builder.Append($@"
public sealed class {className} : {BuildBaseClass()}
{{
    public static class {ProjectReflection.SummerRestConfiguration.Request.Keys} 
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
    public static class {ProjectReflection.SummerRestConfiguration.Request.Values} {{
        public const string Url = {Url.ToEmbeddedString()};
        public const string UrlWithParams = {UrlWithParams.ToEmbeddedString()};
        public const string UrlFormatPattern = {urlFormat};
        public const string SerializedBody = {serializedValue};
        public static readonly ContentType? ContentType = {contentTypeConstValue};
        public static class UrlFormat {{
            {urlFormatValues}
        }}
        public static class Headers {{
            {headerValues}
        }}
        public static class Params {{
            {paramsValues}
        }}
        public static class MultipartFormSections {{
            {multipartFormValues}
        }}
    }}
    public {className}() : base(
            {ProjectReflection.SummerRestConfiguration.Request.Values}.Url, 
            {ProjectReflection.SummerRestConfiguration.Request.Values}.UrlWithParams, 
            {ProjectReflection.SummerRestConfiguration.Request.Values}.UrlFormatPattern, {urlFormatValuesArr}, {authProp}) 
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
            if (TypeName == nameof(ProjectReflection.SummerRestConfiguration.Request))
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