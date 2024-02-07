
#if SUMMER_REST_TASK
using SummerRest.Runtime.RequestAdaptor;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace SummerRest.Runtime.Requests
{
    public partial class BaseRequest<TRequest> 
    {
        /// <summary>
        /// Make an async <see cref="Texture2D"/> request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="readable">Texture response readable</param>
        /// <returns></returns>
        public UniTask<Texture2D> TextureRequestAsync(bool readable)
        {
            return WebRequestUtility.TextureRequestAsync(AbsoluteUrl, readable, SetRequestData);
        }        
        /// <summary>
        /// Make an async <see cref="AudioClip"/> request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="audioType">Type of the audio response</param>
        /// <returns></returns>
        public UniTask<AudioClip> AudioRequestAsync(AudioType audioType)
        {
            return WebRequestUtility.AudioRequestAsync(AbsoluteUrl, audioType, SetRequestData);
        }        
  
        /// <summary>
        /// Make a detailed async <see cref="Texture2D"/> request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="readable">Texture response readable</param>
        /// <returns><see cref="WebResponse{TBody}"/></returns>
        public UniTask<WebResponse<Texture2D>> DetailedTextureRequestAsync(bool readable)
        {
            return WebRequestUtility.DetailedTextureRequestAsync(AbsoluteUrl, readable, SetRequestData);
        }        
        
        /// <summary>
        /// Make a detailed async <see cref="AudioClip"/> request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="audioType">Type of the audio response</param>
        /// <returns><see cref="WebResponse{TBody}"/></returns>
        public UniTask<WebResponse<AudioClip>> DetailedAudioRequestAsync(AudioType audioType)
        {
            return WebRequestUtility.DetailedAudioRequestAsync(AbsoluteUrl, audioType, SetRequestData);
        }
    }
}
#endif
