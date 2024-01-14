
#if SUMMER_REST_TASK
using System;
using SummerRest.Runtime.RequestAdaptor;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

  namespace SummerRest.Runtime.Request
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
        public UniTask<TResponse> RequestAsync<TResponse>()
        {
            using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
            return RequestAsync(request);
        }        
        public UniTask<UnityWebRequest> RequestFromUnityWebRequestAsync(UnityWebRequest webRequest)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetFromUnityWebRequest(webRequest);
            return RequestAsync(request);
        }     
        public UniTask<Texture2D> TextureRequestAsync(bool readable)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetTextureRequest(AbsoluteUrl, readable);
            return RequestAsync(request);
        }        
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
        public UniTask<WebResponse<TResponse>> DetailedRequestAsync<TResponse>()
        {
            using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
            return DetailedRequestAsync(request);
        }        
        public UniTask<WebResponse<UnityWebRequest>> DetailedRequestFromUnityWebRequestAsync(UnityWebRequest webRequest)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetFromUnityWebRequest(webRequest);
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
    }
}
#endif
