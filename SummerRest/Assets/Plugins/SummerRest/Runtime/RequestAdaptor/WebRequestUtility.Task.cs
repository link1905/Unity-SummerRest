﻿#if SUMMER_REST_TASK
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    public static partial class WebRequestUtility
    {
        private static async UniTask<TResponse> RequestAsync<TResponse>(
            IWebRequestAdaptor<TResponse> request, Action<IWebRequestAdaptor<TResponse>> adaptorBuilder)
        {
            await SetRequestDataAndWait(request, adaptorBuilder);
            if (request.IsError(out var msg))
                throw new ResponseErrorException(msg);
            return request.ResponseData;
        }
        /// <summary>
        /// Make an async <see cref="Texture2D"/> request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <returns></returns>
        public static UniTask<Texture2D> TextureRequestAsync(
            string url, 
            bool nonReadable,
            Action<IWebRequestAdaptor<Texture2D>> adaptorBuilder = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(url, nonReadable);
            return RequestAsync(request, adaptorBuilder);
        }        
        /// <summary>
        /// Make an async <see cref="AudioClip"/> request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="audioType">Type of the audio response</param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <returns></returns>
        public static UniTask<AudioClip> AudioRequestAsync(
            string url, 
            AudioType audioType,
            Action<IWebRequestAdaptor<AudioClip>> adaptorBuilder = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(url, audioType);
            return RequestAsync(request, adaptorBuilder);
        }        

        /// <summary>
        /// Make an async data request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="method">Http method of the request <see cref="HttpMethod"/></param>
        /// <param name="data">Serialized body of the request, please refer <see cref="IDataSerializer"/> to serialize your data manually</param>
        /// <param name="contentType">Content type of the request <see cref="ContentType"/></param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns></returns>
        public static UniTask<TResponse> DataRequestAsync<TResponse>(
            string url, HttpMethod method, 
            string data = null, ContentType? contentType = null,
            Action<IWebRequestAdaptor<TResponse>> adaptorBuilder = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(url, method, 
                    data, contentType?.FormedContentType);
            return RequestAsync(request, adaptorBuilder);
        }
        
        /// <summary>
        /// Make an async data request that uploads multipart file sections <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="method">Http method of the request <see cref="HttpMethod"/></param>
        /// <param name="data">The form body of the request <seealso cref="MultipartFormDataSection"/> <see cref="MultipartFormFileSection"/></param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns></returns>
        public static UniTask<TResponse> MultipartDataRequestAsync<TResponse>(
            string url, HttpMethod method,             
            List<IMultipartFormSection> data,
            Action<IWebRequestAdaptor<TResponse>> adaptorBuilder = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetMultipartFileRequest<TResponse>(url, method, data);
            return RequestAsync(request, adaptorBuilder);
        }

        
        private static async UniTask<WebResponse<TResponse>> DetailedRequestAsync<TResponse>(
            IWebRequestAdaptor<TResponse> request, Action<IWebRequestAdaptor<TResponse>> adaptorBuilder)
        {
            await SetRequestDataAndWait(request, adaptorBuilder);
            if (request.IsError(out var msg))
                throw new ResponseErrorException(msg);
            return request.WebResponse;
        }
  
        /// <summary>
        /// Make a detailed async <see cref="Texture2D"/> request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="nonReadable">If true, the texture's raw data will not be accessible to script. This can conserve memory. Default: false.</param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <returns><see cref="WebResponse{TBody}"/></returns>
        public static UniTask<WebResponse<Texture2D>> DetailedTextureRequestAsync(string url, 
            bool nonReadable,
            Action<IWebRequestAdaptor<Texture2D>> adaptorBuilder = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(url, nonReadable);
            return DetailedRequestAsync(request, adaptorBuilder);
        }        
        /// <summary>
        /// Make a detailed async <see cref="AudioClip"/> request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="audioType">Type of the audio response</param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <returns><see cref="WebResponse{TBody}"/></returns>
        public static UniTask<WebResponse<AudioClip>> DetailedAudioRequestAsync(
            string url,
            AudioType audioType,
            Action<IWebRequestAdaptor<AudioClip>> adaptorBuilder = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(url, audioType);
            return DetailedRequestAsync(request, adaptorBuilder);
        }        
        
        /// <summary>
        /// Make an async data request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="method">Http method of the request <see cref="HttpMethod"/></param>
        /// <param name="data">Serialized body of the request, please refer <see cref="IDataSerializer"/> to serialize your data manually</param>
        /// <param name="contentType">Content type of the request <see cref="ContentType"/></param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns><see cref="WebResponse{TBody}"/></returns>
        public static UniTask<WebResponse<TResponse>> DetailedDataRequestAsync<TResponse>(
            string url, HttpMethod method, 
            string data = null, ContentType? contentType = null,
            Action<IWebRequestAdaptor<TResponse>> adaptorBuilder = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(url, method, 
                    data, contentType?.FormedContentType);
            return DetailedRequestAsync(request, adaptorBuilder);
        }
        
        /// <summary>
        /// Make an async data request that uploads multipart file sections <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="url">Resource absolute url</param>
        /// <param name="method">Http method of the request <see cref="HttpMethod"/></param>
        /// <param name="data">The form body of the request <seealso cref="MultipartFormDataSection"/> <see cref="MultipartFormFileSection"/></param>
        /// <param name="adaptorBuilder">Used for modifying the request's metrics <see cref="IWebRequestAdaptor{TResponse}"/></param>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns><see cref="WebResponse{TBody}"/></returns>
        public static UniTask<WebResponse<TResponse>> DetailedMultipartDataRequestAsync<TResponse>(
            string url, HttpMethod method,      
            List<IMultipartFormSection> data,
            Action<IWebRequestAdaptor<TResponse>> adaptorBuilder = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetMultipartFileRequest<TResponse>(url, method, data);
            return DetailedRequestAsync(request, adaptorBuilder);
        }
    }
}
#endif