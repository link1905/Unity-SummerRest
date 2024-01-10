using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Utilities.RequestComponents;
using UnityEngine;

namespace SummerRest.Runtime.Request
{
    public abstract class BaseRequest<TRequest> : IWebRequest where TRequest : BaseRequest<TRequest>, new() 
    {
        public string AbsoluteUrl { get; protected set; }
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

        protected virtual void SetRequestData<TResponse>(IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            if (RedirectLimit.HasValue)
                requestAdaptor.RedirectLimit = RedirectLimit.Value;
            if (TimeoutSeconds.HasValue)
                requestAdaptor.TimeoutSeconds = TimeoutSeconds.Value;
            if (ContentType is not null)
                requestAdaptor.ContentType = ContentType;
            foreach (var (k, v) in Headers)
                requestAdaptor.SetHeader(k, v);
        }
        private IEnumerator SetRequestDataAndWait<TResponse>(
            IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            SetRequestData(requestAdaptor);
            yield return requestAdaptor.RequestInstruction;
        }

        #region Coroutine
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

        protected IEnumerator RequestCoroutine<TResponse>(
            IWebRequestAdaptor<TResponse> request,
            Action<TResponse> doneCallback, Action<string> errorCallback)
        {
            yield return SetRequestDataAndWait(request);
            if (!HandleError(request, errorCallback))
                doneCallback?.Invoke(request.ResponseData);
        }
        protected IEnumerator TextureRequestCoroutine(Action<Texture2D> doneCallback,
            bool readable, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }
        protected IEnumerator AudioRequestCoroutine(Action<AudioClip> doneCallback,
            AudioType audioType, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }
        protected IEnumerator RequestCoroutine<TResponse>(Action<TResponse> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }
        protected IEnumerator DetailedRequestCoroutine<TResponse>(IWebRequestAdaptor<TResponse> request,
            Action<WebResponse<TResponse>> doneCallback, Action<string> errorCallback)
        {
            yield return SetRequestDataAndWait(request);
            if (!HandleError(request, errorCallback))
                doneCallback?.Invoke(request.WebResponse);
        }
        protected IEnumerator DetailedTextureRequestCoroutine(Action<WebResponse<Texture2D>> doneCallback,
            bool readable, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }

        protected IEnumerator DetailedAudioRequestCoroutine(Action<WebResponse<AudioClip>> doneCallback,
            AudioType audioType, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }

        protected IEnumerator DetailedRequestCoroutine<TBody>(Action<WebResponse<TBody>> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TBody>(AbsoluteUrl, Method, SerializedBody);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }

        // protected IEnumerator AuthRequestCoroutine<TResponse, TAuthAppender>(
        //     IWebRequestAdaptor<TResponse> request,
        //     Action<TResponse> doneCallback, Action<string> errorCallback) where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     yield return SetAuthRequestDataAndWait<TResponse, TAuthAppender>(request);
        //     if (!HandleError(request, errorCallback))
        //         doneCallback?.Invoke(request.ResponseData);
        // }
        // protected IEnumerator AuthRequestCoroutine<TResponse, TAuthAppender>(Action<TResponse> doneCallback,
        //     Action<string> errorCallback = null) where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     using var request =
        //         IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
        //     yield return AuthRequestCoroutine<TResponse, TAuthAppender>(request, doneCallback, errorCallback);
        // }
        // protected IEnumerator TextureAuthRequestCoroutine<TAuthAppender>(Action<Texture2D> doneCallback,
        //     bool readable, Action<string> errorCallback = null) where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
        //     yield return AuthRequestCoroutine<Texture2D, TAuthAppender>(request, doneCallback, errorCallback);
        // }
        // protected IEnumerator AudioAuthRequestCoroutine<TAuthAppender>(Action<AudioClip> doneCallback,
        //     AudioType audioType, Action<string> errorCallback = null) where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
        //     yield return AuthRequestCoroutine<AudioClip, TAuthAppender>(request, doneCallback, errorCallback);
        // }
        //
        
        // protected IEnumerator DetailedAuthRequestCoroutine<TResponse, TAuthAppender>(IWebRequestAdaptor<TResponse> request,
        //     Action<WebResponse<TResponse>> doneCallback, Action<string> errorCallback) where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     yield return SetAuthRequestDataAndWait<TResponse, TAuthAppender>(request);
        //     if (!HandleError(request, errorCallback))
        //         doneCallback?.Invoke(request.WebResponse);
        // }
        // protected IEnumerator DetailedTextureAuthRequestCoroutine<TAuthAppender>(Action<WebResponse<Texture2D>> doneCallback,
        //     bool readable, Action<string> errorCallback = null) where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
        //     yield return DetailedAuthRequestCoroutine<Texture2D, TAuthAppender>(request, doneCallback, errorCallback);
        // }
        // protected IEnumerator DetailedAudioRequestCoroutine<TAuthAppender>(Action<WebResponse<AudioClip>> doneCallback,
        //     AudioType audioType, Action<string> errorCallback = null) where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
        //     yield return DetailedAuthRequestCoroutine<AudioClip, TAuthAppender>(request, doneCallback, errorCallback);
        // }
        // protected IEnumerator DetailedAuthRequestCoroutine<TResponse, TAuthAppender>(Action<WebResponse<TResponse>> doneCallback,
        //     Action<string> errorCallback = null) where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     using var request =
        //         IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
        //     yield return DetailedAuthRequestCoroutine<TResponse, TAuthAppender>(request, doneCallback, errorCallback);
        // }

        #endregion

        #if !SUMMER_REST_TASK
        protected async UniTask<TResponse> RequestAsync<TResponse>(
            IWebRequestAdaptor<TResponse> request)
        {
            await SetRequestDataAndWait(request);
            if (request.IsError(out var msg))
                throw new Exception(msg);
            return request.ResponseData;
        }
        protected UniTask<TResponse> RequestAsync<TResponse>()
        {
            using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
            return RequestAsync(request);
        }        
        protected UniTask<Texture2D> TextureRequestAsync(bool readable)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            return RequestAsync(request);
        }        
        protected UniTask<AudioClip> AudioRequestAsync(AudioType audioType)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            return RequestAsync(request);
        }        
        
        // protected async UniTask<TResponse> AuthRequestAsync<TResponse, TAuthAppender>(
        //     IWebRequestAdaptor<TResponse> request) where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     await SetAuthRequestDataAndWait<TResponse, TAuthAppender>(request);
        //     if (request.IsError(out var msg))
        //         throw new Exception(msg);
        //     return request.ResponseData;
        // }
        // protected UniTask<TResponse> AuthRequestAsync<TResponse, TAuthAppender>() where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
        //     return AuthRequestAsync<TResponse, TAuthAppender>(request);
        // }        
        // protected UniTask<Texture2D> TextureAuthRequestAsync<TAuthAppender>(bool readable) where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
        //     return AuthRequestAsync<Texture2D, TAuthAppender>(request);
        // }        
        // protected UniTask<AudioClip> AudioAuthRequestAsync<TAuthAppender>(AudioType audioType) where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
        //     return AuthRequestAsync<AudioClip, TAuthAppender>(request);
        // }       
        
        protected async UniTask<WebResponse<TResponse>> DetailedRequestAsync<TResponse>(
            IWebRequestAdaptor<TResponse> request)
        {
            await SetRequestDataAndWait(request);
            return request.WebResponse;
        }
        protected UniTask<WebResponse<TResponse>> DetailedRequestAsync<TResponse>()
        {
            using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
            return DetailedRequestAsync(request);
        }        
        protected UniTask<WebResponse<Texture2D>> DetailedTextureRequestAsync(bool readable)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            return DetailedRequestAsync(request);
        }        
        protected UniTask<WebResponse<AudioClip>> DetailedAudioRequestAsync(AudioType audioType)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            return DetailedRequestAsync(request);
        }        
        
        
        // protected async UniTask<WebResponse<TResponse>> DetailedAuthRequestAsync<TResponse, TAuthAppender>(
        //     IWebRequestAdaptor<TResponse> request) where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     await SetAuthRequestDataAndWait<TResponse, TAuthAppender>(request);
        //     return request.WebResponse;
        // }
        // protected UniTask<WebResponse<TResponse>> DetailedRequestAsync<TResponse, TAuthAppender>() 
        //     where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
        //     return DetailedAuthRequestAsync<TResponse, TAuthAppender>(request);
        // }        
        // protected UniTask<WebResponse<Texture2D>> DetailedTextureRequestAsync<TAuthAppender>(bool readable)
        //     where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
        //     return DetailedAuthRequestAsync<Texture2D, TAuthAppender>(request);
        // }        
        // protected UniTask<WebResponse<AudioClip>> DetailedAudioRequestAsync<TAuthAppender>(AudioType audioType)
        //     where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
        // {
        //     using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
        //     return DetailedAuthRequestAsync<AudioClip, TAuthAppender>(request);
        // }
        #endif
    }
}