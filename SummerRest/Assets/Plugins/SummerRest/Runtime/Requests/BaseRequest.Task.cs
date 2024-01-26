
#if SUMMER_REST_TASK
using System;
using SummerRest.Runtime.RequestAdaptor;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using SummerRest.Runtime.Requests;

namespace SummerRest.Runtime.Requests
{
    public partial class BaseRequest<TRequest> 
    {
        
        protected async UniTask<TResponse> RequestAsync<TResponse>(
            IWebRequestAdaptor<TResponse> request)
        {
            await SetRequestDataAndWait(request);
            if (request.IsError(out var msg))
                throw new Exception(msg);
            return request.ResponseData;
        }
        /// <summary>
        /// Make an async data request. Please note that this method throws an exception when encountering issues
        /// </summary>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns></returns>
        public UniTask<TResponse> RequestAsync<TResponse>()
        {
            using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
            return RequestAsync(request);
        }        
        /// <summary>
        /// Make an async request based on an existing <see cref="UnityWebRequest"/>. Please note that this method throws an exception when encountering issues
        /// </summary>
        /// <returns></returns>
        public UniTask<UnityWebRequest> RequestFromUnityWebRequestAsync(UnityWebRequest webRequest)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetFromUnityWebRequest(webRequest);
            return RequestAsync(request);
        }     
        /// <summary>
        /// Make an async <see cref="Texture2D"/> request based on an existing <see cref="UnityWebRequest"/>. Please note that this method throws an exception when encountering issues
        /// </summary>
        /// <param name="readable">Texture response readable</param>
        /// <returns></returns>
        public UniTask<Texture2D> TextureRequestAsync(bool readable)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            return RequestAsync(request);
        }        
        /// <summary>
        /// Make an async <see cref="AudioClip"/> request based on an existing <see cref="UnityWebRequest"/>. Please note that this method throws an exception when encountering issues
        /// </summary>
        /// <param name="audioType">Type of the audio response</param>
        /// <returns></returns>
        public UniTask<AudioClip> AudioRequestAsync(AudioType audioType)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            return RequestAsync(request);
        }        
        
        
        protected async UniTask<WebResponse<TResponse>> DetailedRequestAsync<TResponse>(
            IWebRequestAdaptor<TResponse> request)
        {
            await SetRequestDataAndWait(request);
            return request.WebResponse;
        }
        /// <summary>
        /// Make a detailed async data request
        /// </summary>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns>A response object contains HTTP response's essential fields</returns>
        public UniTask<WebResponse<TResponse>> DetailedRequestAsync<TResponse>()
        {
            using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
            return DetailedRequestAsync(request);
        }        
        /// <summary>
        /// Make a detailed async request based on an existing <see cref="UnityWebRequest"/>
        /// </summary>
        /// <returns>A response object contains HTTP response's essential fields</returns>
        public UniTask<WebResponse<UnityWebRequest>> DetailedRequestFromUnityWebRequestAsync(UnityWebRequest webRequest)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetFromUnityWebRequest(webRequest);
            return DetailedRequestAsync(request);
        }     
        /// <summary>
        /// Make a detailed async <see cref="Texture2D"/> request
        /// </summary>
        /// <param name="readable">Texture response readable</param>
        /// <returns>A response object contains HTTP response's essential fields</returns>
        public UniTask<WebResponse<Texture2D>> DetailedTextureRequestAsync(bool readable)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            return DetailedRequestAsync(request);
        }        
        /// <summary>
        /// Make a detailed async <see cref="AudioClip"/> request
        /// </summary>
        /// <param name="audioType">Type of the audio response</param>
        /// <returns>A response object contains HTTP response's essential fields</returns>
        public UniTask<WebResponse<AudioClip>> DetailedAudioRequestAsync(AudioType audioType)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetAudioRequest(AbsoluteUrl, audioType);
            return DetailedRequestAsync(request);
        }        
    }
}
#endif
