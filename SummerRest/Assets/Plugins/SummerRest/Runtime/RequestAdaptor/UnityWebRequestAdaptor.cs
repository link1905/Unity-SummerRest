using System.Collections;
using System.Net;
using SummerRest.Runtime.Extensions;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.Pool;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Pool;

namespace SummerRest.Runtime.RequestAdaptor
{
    /// <summary>
    /// Default request adaptor wrapping a <see cref="UnityWebRequest"/>
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    internal abstract class UnityWebRequestAdaptor<TSelf, TResponse> :
        IWebRequestAdaptor<TResponse>,
        IPoolable<TSelf, UnityWebRequest> where TSelf : UnityWebRequestAdaptor<TSelf, TResponse>, new()
    {
        internal UnityWebRequest WebRequest { get; private set; }
        /// <summary>
        /// For audio and texture requests, we do not provide raw response for better performance
        /// </summary>
        public virtual string RawResponse => null;
        public TResponse ResponseData { get; private set; }

        public IObjectPool<TSelf> Pool { get; set; }

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

        public bool IsError(out ResponseError error)
        {
            if (WebRequest.result == UnityWebRequest.Result.Success)
            {
                error = default;
                return false;
            }
            error = new ResponseError(WebRequest.error, WebRequest.downloadHandler.text, (HttpStatusCode)WebRequest.responseCode);
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

        private ContentType? _contentType;
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
                SetAdaptedContentType(value);
            }
        }

        protected virtual void SetAdaptedContentType(ContentType? contentType)
        {
            _contentType = contentType;
            if (contentType is null)
            {
                WebRequest.uploadHandler.contentType = null;
                return;
            }
            WebRequest.uploadHandler.contentType = contentType.Value.FormedContentType;
        }

        /// <summary>
        /// Build response based on the type of current adaptor <br/>
        /// </summary>
        /// <returns></returns>
        internal abstract TResponse BuildResponse();

        public WebResponse<TResponse> WebResponse
            => new(WebRequest,
                (HttpStatusCode)WebRequest.responseCode,
                IContentTypeParser.Current.ParseContentTypeFromHeader(
                    WebRequest.GetResponseHeader(IContentTypeParser.Current.ContentTypeHeaderKey)),
                WebRequest.GetResponseHeaders(),
                WebRequest.error,
                RawResponse,
                ResponseData
            );

        public virtual IEnumerator RequestInstruction
        {
            get
            {
                // EditorCoroutine only accepts yield return null
                if (Application.isPlaying)
                {
                    WebRequest.SendWebRequest();
                    while (!WebRequest.isDone)
                        yield return null;
                }
                else
                    yield return WebRequest.SendWebRequest();
                if (WebRequest.result == UnityWebRequest.Result.Success)
                    ResponseData = BuildResponse();
            }
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