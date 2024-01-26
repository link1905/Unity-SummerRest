using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SummerRest.Editor.Attributes;
using SummerRest.Editor.DataStructures;
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
        /// <summary>
        /// Content type of associated requests
        /// </summary>
        [SerializeField, JsonIgnore, InheritOrCustom(InheritChoice.Auto | InheritChoice.Custom)]
        private InheritOrCustomContainer<ContentType> contentType;
        public ContentType? ContentType { get; private set; }
        
        [SerializeField, JsonIgnore] private HttpMethod method;
        /// <summary>
        /// Will be automatically appended to <see cref="urlWithParam"/>
        /// </summary>
        [SerializeField, JsonIgnore] private KeyValue[] requestParams;
        /// <summary>
        /// Will be deserialized into string in the generating source process <seealso cref="SummerRest.Editor.Models.RequestBody"/>
        /// </summary>
        [SerializeField, JsonIgnore] private RequestBody requestBody;
        [SerializeField, JsonIgnore] private string urlWithParam;
        public string UrlWithParams => urlWithParam;
        public HttpMethod Method
        {
            get => method;
            private set => method = value;
        }
        public KeyValue[] RequestParams
        {
            get => requestParams;
            private set => requestParams = value;
        }

        public RequestBody RequestBody
        {
            get => requestBody;
            private set => requestBody = value;
        }

        public string SerializedBody => RequestBody.SerializedData(DataFormat, false);
        public KeyValue[] SerializedForm => RequestBody.SerializedForm;
        public bool IsMultipart => requestBody.IsMultipart;

        private IEnumerable<KeyValuePair<string, string>> Params => requestParams?.Select(e => (KeyValuePair<string, string>)e);
        public override void CacheValues()
        {
            base.CacheValues();
            urlWithParam = DefaultUrlBuilder.BuildUrl(Url, Params);
            var contentTypeCache = contentType.Cache(Parent,
                allow: InheritChoice.Auto | InheritChoice.Custom,
                defaultWhenInvalid: InheritChoice.Auto,
                whenInherit: null, whenAuto: () =>
                {
                    var builtContentType = requestBody.CacheValue(DataFormat);
                    return builtContentType.HasValue ? new Present<ContentType>(builtContentType.Value) : Present<ContentType>.Absent;
                });
            ContentType = contentTypeCache.HasValue ? contentTypeCache.Value : null;
        }

        public override void Delete(bool fromParent)
        {
            if (fromParent && Parent is EndpointContainer parent)
                parent.Requests.Remove(this);
            base.Delete(fromParent);
        }

        /// <summary>
        /// Caches latest response of this request (editor-only)
        /// </summary>
        [SerializeField, JsonIgnore] private Response latestResponse;
        [JsonIgnore] public Response LatestResponse
        {
            get => latestResponse;
            set => latestResponse = value;
        }
        public override string TypeName => nameof(Request);
    }
}
