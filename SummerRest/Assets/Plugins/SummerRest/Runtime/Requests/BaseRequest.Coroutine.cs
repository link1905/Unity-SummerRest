using System;
using System.Collections;
using System.Collections.Generic;
using SummerRest.Runtime.Extensions;
using SummerRest.Runtime.RequestAdaptor;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.Requests
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
        /// <summary>
        /// Simple data request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns></returns>
        public IEnumerator RequestCoroutine<TResponse>(Action<TResponse> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }
        
        /// <summary>
        /// Simple request based on an existing <see cref="UnityWebRequest"/> <br/>
        /// </summary>
        /// <param name="webRequest">Wrapped request</param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <returns></returns>
        public IEnumerator RequestCoroutineFromUnityWebRequest(UnityWebRequest webRequest, Action<UnityWebRequest> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetFromUnityWebRequest(webRequest);
            // Make sure the request is alighted with current properties
            webRequest.url = AbsoluteUrl;
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }
        /// <summary>
        /// Simple <see cref="Texture2D"/> request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="readable">Texture response readable</param>
        /// <returns></returns>
        public IEnumerator TextureRequestCoroutine(Action<Texture2D> doneCallback,
            bool readable, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }
        /// <summary>
        /// Simple <see cref="AudioClip"/> request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="audioType">Type of the audio response</param>
        /// <returns></returns>
        public IEnumerator AudioRequestCoroutine(Action<AudioClip> doneCallback,
            AudioType audioType, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }

        /// <summary>
        /// Simple data request that uploads multipart file sections (the boundary leverages <see cref="ContentType"/>)
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="multipartFormSections">Multipart file sections <see cref="MultipartFormFileSection"/></param>
        /// <returns></returns>
        public IEnumerator MultipartFileRequestCoroutine<TResponse>(Action<TResponse> doneCallback,
            List<IMultipartFormSection> multipartFormSections,
            Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetMultipartFileRequest<TResponse>(AbsoluteUrl, multipartFormSections);
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }

        protected IEnumerator DetailedRequestCoroutine<TResponse>(IWebRequestAdaptor<TResponse> request,
            Action<WebResponse<TResponse>> doneCallback, Action<string> errorCallback)
        {
            yield return SetRequestDataAndWait(request);
            if (!HandleError(request, errorCallback))
                doneCallback?.Invoke(request.WebResponse);
        }
        /// <summary>
        /// Detailed data request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        public IEnumerator DetailedRequestCoroutine<TResponse>(Action<WebResponse<TResponse>> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }
        /// <summary>
        /// Detailed request based on an existing <see cref="UnityWebRequest"/> <br/>
        /// </summary>
        /// <param name="webRequest">Wrapped request</param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <returns></returns>
        public IEnumerator DetailedRequestCoroutineFromUnityWebRequest(UnityWebRequest webRequest, Action<WebResponse<UnityWebRequest>> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetFromUnityWebRequest(webRequest);
            webRequest.url = AbsoluteUrl;
            webRequest.method = Method.ToUnityHttpMethod();
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }
        /// <summary>
        /// Detailed <see cref="Texture2D"/> request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="readable">Texture response readable</param>
        /// <returns></returns>
        public IEnumerator DetailedTextureRequestCoroutine(Action<WebResponse<Texture2D>> doneCallback,
            bool readable, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }
        /// <summary>
        /// Detailed <see cref="AudioClip"/> request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="audioType">Type of the audio response</param>
        /// <returns></returns>
        public IEnumerator DetailedAudioRequestCoroutine(Action<WebResponse<AudioClip>> doneCallback,
            AudioType audioType, Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }
        
        /// <summary>
        /// Detailed data request that uploads multipart file sections (the boundary leverages <see cref="ContentType"/>)
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="multipartFormSections">Multipart file sections <see cref="MultipartFormFileSection"/></param>
        /// <returns></returns>
        public IEnumerator DetailedMultipartFileRequestCoroutine<TResponse>(Action<WebResponse<TResponse>> doneCallback,
            List<IMultipartFormSection> multipartFormSections,
            Action<string> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetMultipartFileRequest<TResponse>(AbsoluteUrl, multipartFormSections);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }
    }
}