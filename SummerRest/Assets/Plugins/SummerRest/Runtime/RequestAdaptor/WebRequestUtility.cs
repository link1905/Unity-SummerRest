using System;
using System.Collections;
using System.Collections.Generic;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    /// <summary>
    /// Contains methods for creating random requests which have no predefined structure in the plugin 
    /// </summary>
    public static partial class WebRequestUtility
    {
        private static IEnumerator SetRequestDataAndWait<TResponse>(IWebRequestAdaptor<TResponse> request,
            Action<IWebRequestAdaptor<TResponse>> adaptorBuilder)
        {
            adaptorBuilder?.Invoke(request);
            return request.RequestInstruction;
        }

        private static bool HandleError<TResponse>(
            IWebRequestAdaptor<TResponse> request, Action<ResponseError> errorCallback)
        {
            var error = request.IsError(out var msg);
            if (error)
            {
                if (errorCallback is not null)
                    errorCallback(msg);
                else
                    Debug.LogErrorFormat(
                        @"There was an missed error ""{0}"" when trying to access the resource ""{1}"". Please provide the errorCallback to catch it",
                        msg.Message, request.Url);
            }

            return error;
        }
        internal static IEnumerator RequestCoroutine<TResponse>(
            IWebRequestAdaptor<TResponse> request,
            Action<TResponse> doneCallback, 
            Action<ResponseError> errorCallback,
            Action<IWebRequestAdaptor<TResponse>> adaptorBuilder)
        {
            yield return SetRequestDataAndWait(request, adaptorBuilder);
            if (!HandleError(request, errorCallback))
                doneCallback?.Invoke(request.ResponseData);
        }

        /// <summary>
        /// Simple <see cref="Texture2D"/> request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <returns></returns>
        public static IEnumerator TextureRequestCoroutine(string url, 
            bool nonReadable, 
            Action<Texture2D> doneCallback, 
            Action<ResponseError> errorCallback = null, 
            Action<IWebRequestAdaptor<Texture2D>> adaptorBuilder = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(url, nonReadable);
            yield return RequestCoroutine(request, doneCallback, errorCallback, adaptorBuilder);
        }
        /// <summary>
        /// Simple <see cref="AudioClip"/> request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="audioType">Type of the audio response</param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <returns></returns>
        public static IEnumerator AudioRequestCoroutine(string url, AudioType audioType,
            Action<AudioClip> doneCallback, 
            Action<ResponseError> errorCallback = null, 
            Action<IWebRequestAdaptor<AudioClip>> adaptorBuilder = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(url, audioType);
            yield return RequestCoroutine(request, doneCallback, errorCallback, adaptorBuilder);
        }

        /// <summary>
        /// Simple data request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="method">Http method of the request <see cref="HttpMethod"/></param>
        /// <param name="data">Serialized body of the request, please refer <see cref="IDataSerializer"/> to serialize your data manually</param>
        /// <param name="contentType">Content type of the request <see cref="ContentType"/></param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns></returns>
        public static IEnumerator DataRequestCoroutine<TResponse>(
            string url, HttpMethod method, 
            Action<TResponse> doneCallback,
            string data = null, ContentType? contentType = null,
            Action<ResponseError> errorCallback = null, 
            Action<IWebRequestAdaptor<TResponse>> adaptorBuilder = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(url, method, 
                    data, contentType);
            yield return RequestCoroutine(request, doneCallback, errorCallback, adaptorBuilder);
        }

        /// <summary>
        /// Simple data request that uploads multipart file sections
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="method">Http method of the request <see cref="HttpMethod"/></param>
        /// <param name="data">The form body of the request <seealso cref="MultipartFormDataSection"/> <see cref="MultipartFormFileSection"/></param>
        /// <param name="contentType">Content type of the sections</param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <returns></returns>
        public static IEnumerator MultipartDataRequestCoroutine<TResponse>(
            string url,
            HttpMethod method,
            List<IMultipartFormSection> data,
            ContentType? contentType,
            Action<TResponse> doneCallback,
            Action<ResponseError> errorCallback = null,
            Action<IWebRequestAdaptor<TResponse>> adaptorBuilder = null)
        {
            using var request = IWebRequestAdaptorProvider.Current
                .GetMultipartFileRequest<TResponse>(url, method, data, contentType);
            yield return RequestCoroutine(request, doneCallback, errorCallback, adaptorBuilder);
        }
        
        
        private static IEnumerator DetailedRequestCoroutine<TResponse>(IWebRequestAdaptor<TResponse> request,
            Action<WebResponse<TResponse>> doneCallback, 
            Action<ResponseError> errorCallback,
            Action<IWebRequestAdaptor<TResponse>> adaptorBuilder)
        {
            yield return SetRequestDataAndWait(request, adaptorBuilder);
            if (!HandleError(request, errorCallback))
                doneCallback?.Invoke(request.WebResponse);
        }
        /// <summary>
        /// Detailed <see cref="Texture2D"/> request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <returns></returns>
        public static IEnumerator DetailedTextureRequestCoroutine(
            string url,
            bool nonReadable,
            Action<WebResponse<Texture2D>> doneCallback,
            Action<ResponseError> errorCallback = null,
            Action<IWebRequestAdaptor<Texture2D>> adaptorBuilder = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(url, nonReadable);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback, adaptorBuilder);
        }

        /// <summary>
        /// Detailed <see cref="AudioClip"/> request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="audioType">Type of the audio response</param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <returns></returns>
        public static IEnumerator DetailedAudioRequestCoroutine(
            string url,
            AudioType audioType,
            Action<WebResponse<AudioClip>> doneCallback,
            Action<ResponseError> errorCallback = null,
            Action<IWebRequestAdaptor<AudioClip>> adaptorBuilder = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(url, audioType);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback, adaptorBuilder);
        }

        /// <summary>
        /// Detailed data request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="method">Http method of the request <see cref="HttpMethod"/></param>
        /// <param name="data">Serialized body of the request, please refer <see cref="IDataSerializer"/> to serialize your data manually</param>
        /// <param name="contentType">Content type of the request <see cref="ContentType"/></param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns></returns>
        public static IEnumerator DetailedDataRequestCoroutine<TResponse>(
            string url, HttpMethod method, 
            Action<WebResponse<TResponse>> doneCallback,
            string data = null, ContentType? contentType = null,
            Action<ResponseError> errorCallback = null,
            Action<IWebRequestAdaptor<TResponse>> adaptorBuilder = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>
                    (url, method, data, contentType);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback, adaptorBuilder);
        }
        
        /// <summary>
        /// Detailed data request that uploads multipart file sections
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="method">Http method of the request <see cref="HttpMethod"/></param>
        /// <param name="data">The form body of the request <seealso cref="MultipartFormDataSection"/> <see cref="MultipartFormFileSection"/></param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <param name="contentType">Content type of the sections</param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <returns></returns>
        public static IEnumerator DetailedMultipartDataRequestCoroutine<TResponse>(
            string url,
            HttpMethod method,
            List<IMultipartFormSection> data,
            ContentType? contentType,
            Action<WebResponse<TResponse>> doneCallback,
            Action<ResponseError> errorCallback = null,
            Action<IWebRequestAdaptor<TResponse>> adaptorBuilder = null)
        {
            using var request = IWebRequestAdaptorProvider.Current
                .GetMultipartFileRequest<TResponse>(url, method, data, contentType);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback, adaptorBuilder);
        }
    }
}