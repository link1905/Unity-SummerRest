using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Utilities.RequestComponents;
using UnityEngine;

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
        public string AuthKey { get; set; }
        public IAuthAppender AuthAppender { get; set; }
        public ContentType? ContentType { get; set; }
        public virtual string SerializedBody => null;

        protected BaseRequest(string url, string absoluteUrl)
        {
            _url = url;
            AbsoluteUrl = absoluteUrl;
        }

        public static TRequest Create()
        {
            var request = new TRequest();
            return request;
        }

        protected void Init()
        {
            Params.OnChangedParams += RebuildUrl;
        }

        // Gen this constructor
        protected void RebuildUrl()
        {
            AbsoluteUrl = IUrlBuilder.Current.BuildUrl(_url, Params.ParamMapper);
        }

        private void SetAuthData<TResponse>(
            IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            if (string.IsNullOrEmpty(AuthKey))
                return;
            var appender = AuthAppender ?? IAuthAppender.Current;
            appender.Append(AuthKey, requestAdaptor);
        }

        private void SetRequestData<TResponse>(IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            if (RedirectLimit.HasValue)
                requestAdaptor.RedirectLimit = RedirectLimit.Value;
            if (TimeoutSeconds.HasValue)
                requestAdaptor.TimeoutSeconds = TimeoutSeconds.Value;
            if (ContentType is not null)
                requestAdaptor.ContentType = ContentType;
            foreach (var (k, v) in Headers)
                requestAdaptor.SetHeader(k, v);
            SetAuthData(requestAdaptor);
        }

        #region Coroutine

        private IEnumerator SetRequestDataAndWait<TResponse>(
            IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            SetRequestData(requestAdaptor);
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
                    Debug.LogErrorFormat(
                        @"There was an missed error ""{0}"" when trying to access the resource {1}. Please give errorCallback to catch it",
                        msg, AbsoluteUrl);
            }

            return error;
        }

        private IEnumerator SimpleRequestCoroutine<TResponse>(
            IWebRequestAdaptor<TResponse> request,
            Action<TResponse> doneCallback, Action<string> errorCallback)
        {
            yield return SetRequestDataAndWait(request);
            if (!HandleError(request, errorCallback))
                doneCallback?.Invoke(request.ResponseData);
        }

        // public IEnumerator SimpleRequestCoroutine<TResponse>(UnityWebRequest webRequest,
        //     Action<TResponse> doneCallback, Action<string> errorCallback = null)
        // {
        //     using var request = IWebRequestAdaptorProvider.Current.GetFromUnityWebRequest<TResponse>(webRequest);
        //     request.Url = AbsoluteUrl;
        //     request.Method = Method;
        //     yield return SimpleRequestCoroutine(request, doneCallback, errorCallback);
        // }
        public IEnumerator SimpleRequestCoroutine<TBody>(Action<TBody> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TBody>(AbsoluteUrl, Method, SerializedBody);
            yield return SimpleRequestCoroutine(request, doneCallback, errorCallback);
        }
        public IEnumerator SimpleTextureRequestCoroutine(Action<Texture2D> doneCallback,
            bool readable, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            yield return SimpleRequestCoroutine(request, doneCallback, errorCallback);
        }
        public IEnumerator SimpleAudioRequestCoroutine(Action<AudioClip> doneCallback,
            AudioType audioType, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            yield return SimpleRequestCoroutine(request, doneCallback, errorCallback);
        }


        private IEnumerator DetailedRequestCoroutine<TResponse>(IWebRequestAdaptor<TResponse> request,
            Action<WebResponse<TResponse>> doneCallback, Action<string> errorCallback)
        {
            yield return SetRequestDataAndWait(request);
            if (!HandleError(request, errorCallback))
                doneCallback?.Invoke(request.WebResponse);
        }

        // public IEnumerator DetailedRequestCoroutine<TResponse>(UnityWebRequest webRequest,
        //     Action<WebResponse<TResponse>> doneCallback, Action<string> errorCallback = null)
        // {
        //     using var request = IWebRequestAdaptorProvider.Current.GetFromUnityWebRequest<TResponse>(webRequest);
        //     yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        // }

        public IEnumerator DetailedTextureRequestCoroutine(Action<WebResponse<Texture2D>> doneCallback,
            bool readable, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }

        public IEnumerator DetailedAudioRequestCoroutine(Action<WebResponse<AudioClip>> doneCallback,
            AudioType audioType, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }

        public IEnumerator DetailedRequestCoroutine<TBody>(Action<WebResponse<TBody>> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TBody>(AbsoluteUrl, Method, SerializedBody);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }

        #endregion

        #region Task
        private async UniTask<TResponse> SimpleRequestAsync<TResponse>(
            IWebRequestAdaptor<TResponse> request)
        {
            await SetRequestDataAndWait(request);
            if (request.IsError(out var msg))
                throw new Exception(msg);
            return request.ResponseData;
        }

        public UniTask<TResponse> SimpleRequestAsync<TResponse>()
        {
            using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
            return SimpleRequestAsync(request);
        }        
        public UniTask<Texture2D> SimpleTextureRequestAsync(bool readable)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            return SimpleRequestAsync(request);
        }        
        public UniTask<AudioClip> SimpleAudioRequestAsync(AudioType audioType)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            return SimpleRequestAsync(request);
        }        
        
        private async UniTask<WebResponse<TResponse>> DetailedRequestAsync<TResponse>(
            IWebRequestAdaptor<TResponse> request)
        {
            await SetRequestDataAndWait(request);
            return request.WebResponse;
        }
        public UniTask<WebResponse<TResponse>> DetailedRequestAsync<TResponse>()
        {
            using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
            return DetailedRequestAsync(request);
        }        
        public UniTask<WebResponse<Texture2D>> DetailedTextureRequestAsync(bool readable)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            return DetailedRequestAsync(request);
        }        
        public UniTask<WebResponse<AudioClip>> DetailedAudioRequestAsync(AudioType audioType)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            return DetailedRequestAsync(request);
        }        
        #endregion
    }
}