using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SummerRest.Runtime.Parsers;
using SummerRest.Scripts.Utilities.DataStructures;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    public class Request : Endpoint 
    {
        [SerializeField, JsonIgnore] private HttpMethod method;
        [SerializeField, JsonIgnore] private KeyValue[] requestParams;
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

        public string SerializedBody => RequestBody.SerializedData;

        private IEnumerable<KeyValuePair<string, string>> Params => requestParams?.Select(e => (KeyValuePair<string, string>)e);
        public override void CacheValues()
        {
            urlWithParam = DefaultUrlBuilder.BuildUrl(Url, Params);
            base.CacheValues();
        }
        public override void Delete(bool fromParent)
        {
            if (fromParent && Parent is EndpointContainer parent)
                parent.Requests.Remove(this);
            base.Delete(fromParent);
        }

        [SerializeField, JsonIgnore] private Response latestResponse;
        [JsonIgnore] public Response LatestResponse
        {
            get => latestResponse;
            set => latestResponse = value;
        }
        public override string TypeName => nameof(Request);
    }
}
