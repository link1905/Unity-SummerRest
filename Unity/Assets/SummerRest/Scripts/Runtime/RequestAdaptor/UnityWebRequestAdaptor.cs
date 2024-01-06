using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.Pool;
using SummerRest.Runtime.Request;
using SummerRest.Scripts.Utilities.Extensions;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Pool;

namespace SummerRest.Runtime.RequestAdaptor
{
    public abstract class UnityWebRequestAdaptor<TSelf, TResponse> : 
        IWebRequestAdaptor<TResponse>, IPoolable<TSelf, UnityWebRequest> where TSelf : UnityWebRequestAdaptor<TSelf, TResponse>, new()
    {
        protected UnityWebRequest WebRequest { get; private set; }
        public TResponse ResponseData { get; protected set; }
        public IObjectPool<TSelf> Pool { get; set; }
        public Task<TResponse> RequestAsync { get; }
        public static TSelf Create(UnityWebRequest webRequest)
        {
            return BasePool<TSelf, UnityWebRequest>.Create(webRequest);
        }
        public string Url
        {
            get => WebRequest.url;
            set => WebRequest.url = value;
        }

        public void Init(UnityWebRequest data)
        {
            WebRequest = data;
        }
        public void SetHeader(string key, string value)
        {
            WebRequest.SetRequestHeader(key, value);
        }

        public bool IsError(out string error)
        {
            if (WebRequest.result == UnityWebRequest.Result.Success)
            {
                error = null;
                return false;
            }
            error = $"Error {WebRequest.result}: {WebRequest.error}";
            return true;
        }

        public string GetHeader(string key) => WebRequest.GetRequestHeader(key);

        public HttpMethod Method
        {
            get => HttpMethodExtensions.UnityHttpMethod(WebRequest.method);
            set => WebRequest.method = value.ToUnityHttpMethod();
        }

        public int RedirectLimit
        {
            get => WebRequest.redirectLimit;
            set => WebRequest.redirectLimit = value;
        }
        public int TimeoutSeconds
        {
            get => WebRequest.timeout;
            set => WebRequest.timeout = value;
        }
        private ContentType? _contentType = IContentTypeParser.Current.DefaultContentType; 
        public ContentType? ContentType
        {
            get
            {
                if (WebRequest.uploadHandler is null)
                    return null;
                return _contentType;
            }
            set
            {
                if (WebRequest.uploadHandler is null)
                    return;
                _contentType = value;
                _contentType ??= IContentTypeParser.Current.DefaultContentType;
                WebRequest.uploadHandler.contentType = _contentType.Value.FormedContentType;
            }
        }

        protected abstract void DoneRequest();
        public virtual IWebResponse<TResponse> WebResponse => new UnityWebResponse(WebRequest, ResponseData);
        public virtual IEnumerator RequestInstruction
        {
            get
            {
                yield return WebRequest.SendWebRequest();
                if (WebRequest.result != UnityWebRequest.Result.Success)
                    DoneRequest();
            }
        }
        protected struct UnityWebResponse : IWebResponse<TResponse>
        {
            private readonly UnityWebRequest _webRequest;
            public object WrappedRequest => _webRequest;
            public UnityWebResponse(UnityWebRequest webRequest,
                TResponse response)
            {
                _webRequest = webRequest;
                StatusCode = (HttpStatusCode)_webRequest.responseCode;
                Error = _webRequest.error;
                Data = response;
                _contentType = null;
                _headers = null;
                _rawData = null;
            }
            public UnityWebResponse(UnityWebRequest webRequest, string rawResponse,
                TResponse response)
            {
                _webRequest = webRequest;
                _rawData = rawResponse;
                StatusCode = (HttpStatusCode)_webRequest.responseCode;
                Error = _webRequest.error;
                Data = response;
                _contentType = null;
                _headers = null;
            }
            private readonly string _rawData;
            public string RawData
            {
                get
                {
                    if (_rawData is null)
                        Debug.LogWarningFormat("Raw data is null because you're using a caller method with custom parameter (UnityWebRequest,Texture2D,AudioClip...), so we don't try to read the raw data for better performance. It you want to access them, you should use {0} and parse it as UnityWebRequest (eg. (WrappedRequest as UnityWebRequest).downloadHandler.text)", nameof(WrappedRequest));
                    return _rawData;
                }
            }
            // Apply lazy-loading for some fields are slow to be loaded
            private IEnumerable<KeyValuePair<string, string>> _headers;
            public IEnumerable<KeyValuePair<string, string>> Headers
                => _headers ??= _webRequest.GetResponseHeaders();

            private ContentType? _contentType;
            public ContentType ContentType
                => _contentType ??= IContentTypeParser.Current.ParseContentTypeFromHeader(
                    _webRequest.GetRequestHeader(IContentTypeParser.Current.ContentTypeHeaderKey));
            public HttpStatusCode StatusCode { get; }
            public string Error { get; }
            public TResponse Data { get; }
        }
        public virtual void Dispose()
        {
            WebRequest = default;
            ResponseData = default;
            if (Pool is null || this is not TSelf self)
                return;
            Pool.Release(self);
        }
    }
}