using System;
using System.Collections;
using SummerRest.Runtime.Extensions;
using SummerRest.Runtime.RequestAdaptor;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.Request
{
    public partial class BaseRequest<TRequest>
    {
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
        public IEnumerator RequestCoroutine<TResponse>(Action<TResponse> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }
        public IEnumerator RequestCoroutineFromUnityWebRequest(UnityWebRequest webRequest, Action<UnityWebRequest> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetFromUnityWebRequest(webRequest);
            webRequest.url = AbsoluteUrl;
            webRequest.method = Method.ToUnityHttpMethod();
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }
        public IEnumerator TextureRequestCoroutine(Action<Texture2D> doneCallback,
            bool readable, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }
        public IEnumerator AudioRequestCoroutine(Action<AudioClip> doneCallback,
            AudioType audioType, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }

        protected IEnumerator DetailedRequestCoroutine<TResponse>(IWebRequestAdaptor<TResponse> request,
            Action<WebResponse<TResponse>> doneCallback, Action<string> errorCallback)
        {
            yield return SetRequestDataAndWait(request);
            if (!HandleError(request, errorCallback))
                doneCallback?.Invoke(request.WebResponse);
        }
        public IEnumerator DetailedRequestCoroutineFromUnityWebRequest(UnityWebRequest webRequest, Action<WebResponse<UnityWebRequest>> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetFromUnityWebRequest(webRequest);
            webRequest.url = AbsoluteUrl;
            webRequest.method = Method.ToUnityHttpMethod();
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }
        public IEnumerator DetailedRequestCoroutine<TBody>(Action<WebResponse<TBody>> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TBody>(AbsoluteUrl, Method, SerializedBody);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }
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


    }
}