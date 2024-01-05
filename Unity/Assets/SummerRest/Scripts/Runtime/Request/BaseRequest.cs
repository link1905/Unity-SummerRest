using System;
using System.Collections;
using System.Collections.Generic;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.Request
{
    public abstract class BaseRequest<TRequest> : IWebRequest where TRequest : BaseRequest<TRequest>, new()
    {
        public string AbsoluteUrl { get; private set; }
        private string _url;
        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                RebuildUrl();
            }
        }
        public RequestParamContainer Params { get; } = new();
        public IDictionary<string, string> Headers { get; } = 
            new Dictionary<string, string>();
        public HttpMethod Method { get; set; }
        public int? RedirectLimit { get; set; }
        public int? TimeoutSeconds { get; set; }
        // public IAuthData AuthData { get; set; }
        public ContentType ContentType { get; set; }
        public virtual string SerializedBody => null;
        protected BaseRequest(string url)
        {
            _url = url;
        }
        public static TRequest Create()
        {
            var request = new TRequest();
            return request;
        }
        protected void Init()
        {
            Params.OnChangedParams += RebuildUrl;
            RebuildUrl();
        }
        // Gen this constructor
        protected void RebuildUrl()
        {
            AbsoluteUrl = IUrlBuilder.Current.BuildUrl(_url, Params.ParamMapper);
        }
        private IEnumerator SetRequestDataAndWait<TResponse>(
            IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            if (RedirectLimit.HasValue)
                requestAdaptor.RedirectLimit = RedirectLimit.Value;
            if (TimeoutSeconds.HasValue)
                requestAdaptor.TimeoutSeconds = TimeoutSeconds.Value;
            if (ContentType is not null)
                requestAdaptor.ContentType = ContentType;
            foreach (var (k, v) in Headers)
                requestAdaptor.SetHeader(k, v);
            yield return requestAdaptor.RequestInstruction;
        }
    
        private bool HandleError<TResponse>(
            IWebRequestAdaptor<TResponse> request, Action<string> errorCallback)
        {
            var error = request.IsError(out var msg);
            if (error)
            {
                if (errorCallback is not null)
                    errorCallback(msg);
                else
                    Debug.LogErrorFormat(@"There was an missed error ""{0}"" when trying to access the resource {1}. Please give errorCallback to catch it", msg, AbsoluteUrl);
            }
            return error;
        }
        private IEnumerator SimpleResponseCoroutine<TResponse>(
            IWebRequestAdaptor<TResponse> request, 
            Action<TResponse> doneCallback, Action<string> errorCallback)
        {
            yield return SetRequestDataAndWait(request);
            if (!HandleError(request, errorCallback))
                doneCallback?.Invoke(request.ResponseData);
        }
        public IEnumerator SimpleResponseCoroutine<TResponse>(UnityWebRequest webRequest, 
            Action<TResponse> doneCallback, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetFromUnityWebRequest<TResponse>(webRequest);
            request.Url = AbsoluteUrl;
            request.Method = Method;
            yield return SimpleResponseCoroutine(request, doneCallback, errorCallback);
        }
        public IEnumerator SimpleResponseCoroutine(Action<Texture2D> doneCallback, 
            bool readable, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            yield return SimpleResponseCoroutine(request, doneCallback, errorCallback);
        }
        public IEnumerator SimpleResponseCoroutine(Action<AudioClip> doneCallback, 
            AudioType audioType, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            yield return SimpleResponseCoroutine(request, doneCallback, errorCallback);
        }
        public IEnumerator SimpleResponseCoroutine<TBody>(Action<TBody> doneCallback, 
            Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TBody>(AbsoluteUrl, Method, SerializedBody);
            yield return SimpleResponseCoroutine(request, doneCallback, errorCallback);
        }
    
        private IEnumerator DetailedResponseCoroutine<TResponse>(IWebRequestAdaptor<TResponse> request, 
            Action<IWebResponse<TResponse>> doneCallback, Action<string> errorCallback)
        {
            yield return SetRequestDataAndWait(request);
            if (!HandleError(request, errorCallback))
                doneCallback?.Invoke(request.WebResponse);
        }
        public IEnumerator DetailedResponseCoroutine<TResponse>(UnityWebRequest webRequest, 
            Action<IWebResponse<TResponse>> doneCallback, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetFromUnityWebRequest<TResponse>(webRequest);
            yield return DetailedResponseCoroutine(request, doneCallback, errorCallback);
        }
        public IEnumerator DetailedResponseCoroutine(Action<IWebResponse<Texture2D>> doneCallback, 
            bool readable, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            yield return DetailedResponseCoroutine(request, doneCallback, errorCallback);
        }
        public IEnumerator DetailedResponseCoroutine(Action<IWebResponse<AudioClip>> doneCallback, 
            AudioType audioType, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            yield return DetailedResponseCoroutine(request, doneCallback, errorCallback);
        }
        public IEnumerator DetailedResponseCoroutine<TBody>(Action<IWebResponse<TBody>> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TBody>(AbsoluteUrl, Method, SerializedBody);
            yield return DetailedResponseCoroutine(request, doneCallback, errorCallback);
        }
    }

    public abstract class BaseRequest<TRequest, TRequestBody> : BaseRequest<TRequest>, IWebRequest<TRequestBody>
        where TRequest : BaseRequest<TRequest>, new()
    {
        public TRequestBody BodyData { get; set; }
        public DataFormat BodyFormat { get; set; }

        public override string SerializedBody => BodyData is null ? null : IDataSerializer.Current.Serialize(BodyData, BodyFormat);

        protected BaseRequest(string url) : base(url)
        {
        }
    }
}