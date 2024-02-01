using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SummerRest.Editor.Attributes;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Utilities;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// A request is a terminal callable endpoint type <br/>
    /// It represents a single request with independent values like method, params, body...
    /// </summary>
    public class Request : Endpoint 
    {
        [SerializeField] private PathContainer path;
        public override string Path => path.FinalText;

        public string UrlFormat
        {
            get
            {
                if (path.Count == 0)
                    return string.Empty;
                return $"{Parent.FullPath}/{path.FormatText}";
            }
        }
        public KeyValue[] UrlFormatContainers => path.Containers;

        /// <summary>
        /// Content type of associated requests
        /// </summary>
        [SerializeField, InheritOrCustom(InheritChoice.Auto | InheritChoice.Custom)]
        private InheritOrCustomContainer<ContentType> contentType;
        public ContentType? ContentType { get; set; }
        
        [SerializeField] private HttpMethod method;
        /// <summary>
        /// Will be automatically appended to <see cref="urlWithParam"/>
        /// </summary>
        [SerializeField] private KeyValue[] requestParams;
        /// <summary>
        /// Will be deserialized into string in the generating source process <seealso cref="SummerRest.Editor.Models.RequestBody"/>
        /// </summary>
        [SerializeField] private RequestBody requestBody;
        [SerializeField] private string urlWithParam;
        public string UrlWithParams => urlWithParam;
        public HttpMethod Method => method;
        public KeyValue[] RequestParams => requestParams;
        public RequestBody RequestBody => requestBody;
        
        //Properties for JSON used in source generator
        public DataFormat DataFormat => RequestBody.DataFormat;
        public string SerializedBody => RequestBody.SerializedData(false);
        public KeyValue[] SerializedForm => RequestBody.TextSections;
        public bool IsMultipart => requestBody.IsMultipart;

        public override void CacheValues()
        {
            path.CacheValues();
            base.CacheValues();
            urlWithParam = DefaultUrlBuilder.BuildUrl(Url, requestParams?.Select(e => (KeyValuePair<string, string>)e));
            var contentTypeCache = contentType.Cache(Parent,
                allow: InheritChoice.Auto | InheritChoice.Custom,
                defaultWhenInvalid: InheritChoice.Auto,
                whenInherit: null, whenAuto: () =>
                {
                    var builtContentType = requestBody.CacheValue();
                    return builtContentType.HasValue ? new Present<ContentType>(builtContentType.Value) : Present<ContentType>.Absent;
                });
            ContentType = contentTypeCache.HasValue ? contentTypeCache.Value : null;
        }

        public override void Delete(bool fromParent)
        {
            if (fromParent)
                Parent.Requests.Remove(this);
            base.Delete(fromParent);
        }

        public override void RemoveFormParent()
        {
            if (Parent is null)
                return;
            Parent.Requests.Remove(this);
        }

        /// <summary>
        /// Caches latest response of this request (editor-only)
        /// </summary>
        [SerializeField] private Response latestResponse;
        public Response LatestResponse
        {
            get => latestResponse;
            set => latestResponse = value;
        }
        public override string TypeName => nameof(Request);
        
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString(nameof(Url), Url);
            writer.WriteAttributeString(nameof(UrlFormat), UrlFormat);
            writer.WriteAttributeString(nameof(UrlWithParams), UrlWithParams);
            writer.WriteAttributeString(nameof(Method), Method.ToString());
            if (TimeoutSeconds.HasValue)
                writer.WriteAttributeString(nameof(TimeoutSeconds), TimeoutSeconds.Value.ToString());
            if (RedirectsLimit.HasValue)
                writer.WriteAttributeString(nameof(RedirectsLimit), RedirectsLimit.Value.ToString());
            writer.WriteAttributeString(nameof(DataFormat), DataFormat.ToString());
            writer.WriteAttributeString(nameof(SerializedBody), SerializedBody);
            writer.WriteAttributeString(nameof(IsMultipart), IsMultipart.ToString().ToLower());
            if (ContentType.HasValue)
                writer.WriteObject(nameof(ContentType), ContentType);
            writer.WriteArray(nameof(Headers), Headers);
            writer.WriteArray(nameof(UrlFormatContainers), UrlFormatContainers);
            writer.WriteArray(nameof(RequestParams), RequestParams);
            if (AuthContainer is not null)
                writer.WriteObject(nameof(AuthContainer), AuthContainer);
            writer.WriteArray(nameof(SerializedForm), SerializedForm);
        }
    }
}
