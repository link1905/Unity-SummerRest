using System.Collections.Generic;
using System.Net;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.Pool;
using SummerRest.Runtime.RequestComponents;
using UnityEngine.Networking;
using UnityEngine.Pool;

namespace SummerRest.Runtime.Requests
{
    public class UnityWebResponse<TResponse> : 
        IWebResponse<TResponse>, IPoolable<UnityWebResponse<TResponse>, UnityWebResponse<TResponse>.InitData>
    {
        private UnityWebRequest _request;
        public readonly struct InitData
        {
            public UnityWebRequest UnityWebRequest { get; }
            public string Text { get; }
            public TResponse Response { get; }
            public InitData(UnityWebRequest unityWebRequest, TResponse response, string text)
            {
                UnityWebRequest = unityWebRequest;
                Response = response;
                Text = text;
            }
        }
        public static UnityWebResponse<TResponse> Create(InitData initData)
        {
            return BasePool<UnityWebResponse<TResponse>, InitData>.Create(initData);
        }
        public void Init(InitData init)
        {
            _request = init.UnityWebRequest;
            Data = init.Response;
            _rawText = init.Text;
        }
        public void Dispose()
        {
            _rawText = null;
            Data = default;
            _request?.Dispose();
            Pool?.Release(this);
        }
        public object WrappedRequest => _request;
        public HttpStatusCode StatusCode => 
            (HttpStatusCode)_request.responseCode;
        public ContentType ContentType =>
            IContentTypeParser.Current.ParseContentTypeFromHeader(
                _request.GetResponseHeader(IContentTypeParser.Current.ContentTypeHeaderKey));
        public IEnumerable<KeyValuePair<string, string>> Headers => 
            _request.GetResponseHeaders();
        private string _rawText; 
        public string RawText => _rawText ?? _request.downloadHandler?.text;
        public byte[] RawData => _request.downloadHandler?.data;
        public TResponse Data { get; private set; }
        public IObjectPool<UnityWebResponse<TResponse>> Pool { get; set; }
    }
}