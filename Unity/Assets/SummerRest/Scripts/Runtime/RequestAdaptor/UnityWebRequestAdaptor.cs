using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.Pool;
using SummerRest.Runtime.Request;
using SummerRest.Scripts.Utilities.Extensions;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine.Networking;
using UnityEngine.Pool;

namespace SummerRest.Runtime.RequestAdaptor
{
    internal class UnityWebRequestAdaptor<TResponse> : 
        IWebRequestAdaptor<TResponse>, IPoolable<UnityWebRequestAdaptor<TResponse>, UnityWebRequest>
    {
        private UnityWebRequest _webRequest;
        private string _rawResponse;
        public TResponse ResponseData { get; private set; }
        public IObjectPool<UnityWebRequestAdaptor<TResponse>> Pool { get; set; }
        public Task<TResponse> RequestAsync { get; }
        public static UnityWebRequestAdaptor<TResponse> Create(UnityWebRequest webRequest)
        {
            return BasePool<UnityWebRequestAdaptor<TResponse>, UnityWebRequest>.Create(webRequest);
        }

        public UnityWebRequestAdaptor()
        {
            
        }
        public string Url
        {
            get => _webRequest.url;
            set => _webRequest.url = value;
        }

        public void Init(UnityWebRequest data)
        {
            _webRequest = data;
        }
        public void SetHeader(string key, string value)
        {
            _webRequest.SetRequestHeader(key, value);
        }

        public HttpMethod Method
        {
            get => HttpMethodExtensions.UnityHttpMethod(_webRequest.method);
            set => _webRequest.method = value.ToUnityHttpMethod();
        }

        public int RedirectLimit
        {
            get => _webRequest.redirectLimit;
            set => _webRequest.redirectLimit = value;
        }

        public int TimeoutSeconds
        {
            get => _webRequest.timeout;
            set => _webRequest.timeout = value;
        }

        public ContentType ContentType
        {
            get => new(_webRequest.uploadHandler.contentType);
            set => _webRequest.uploadHandler.contentType = value.FormedContentType;
        }

        public IEnumerator RequestInstruction
        {
            get
            {
                yield return _webRequest.SendWebRequest();
                _rawResponse = _webRequest.downloadHandler.text;
                ResponseData = BuildResponse();
            }
        }

        public IWebResponse<TResponse> WebResponse =>
            new UnityWebResponse(_webRequest, _rawResponse, ResponseData);

        private struct UnityWebResponse : IWebResponse<TResponse>
        {
            private readonly UnityWebRequest _webRequest;

            public UnityWebResponse(UnityWebRequest webRequest, string rawResponse,
                TResponse response)
            {
                _webRequest = webRequest;
                RawData = rawResponse;
                StatusCode = (HttpStatusCode)_webRequest.responseCode;
                Error = _webRequest.error;
                Data = response;
                _contentType = null;
                _headers = null;
            }

            public string RawData { get; }

            // Apply lazy-loading for some fields are slow to be loaded
            private IEnumerable<KeyValuePair<string, string>> _headers;

            public IEnumerable<KeyValuePair<string, string>> Headers
                => _headers ??= _webRequest.GetResponseHeaders();

            private ContentType _contentType;

            public ContentType ContentType
                => _contentType ??= IContentTypeParser.Current.ParseContentTypeFromHeader(
                    _webRequest.GetRequestHeader(IContentTypeParser.Current.ContentTypeHeaderKey));

            public HttpStatusCode StatusCode { get; }
            public string Error { get; }
            public TResponse Data { get; }
        }


        private TResponse BuildResponse()
        {
            if (_webRequest.result != UnityWebRequest.Result.Success)
                return default;
            var parser = IContentTypeParser.Current;
            var contentType =
                parser.ParseDataFormatFromResponse(
                    _webRequest.GetResponseHeader(parser.ContentTypeHeaderKey));
            return IDataSerializer.Current.Deserialize<TResponse>(
                _rawResponse,
                contentType);
        }


        public void Dispose()
        {
            if (Pool is null)
                return;
            _rawResponse = default;
            _webRequest = default;
            ResponseData = default;
            Pool.Release(this);
        }
    }
}