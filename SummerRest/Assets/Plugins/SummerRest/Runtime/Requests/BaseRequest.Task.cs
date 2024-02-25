
#if SUMMER_REST_TASK
using System.Threading;
using SummerRest.Runtime.RequestAdaptor;
using UnityEngine;
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public UniTask<Texture2D> TextureRequestAsync(bool readable, CancellationToken cancellationToken = default)
        {
            return WebRequestUtility.TextureRequestAsync(AbsoluteUrl, readable, SetRequestData, cancellationToken);
        }

        /// <summary>
        /// Make an async <see cref="AudioClip"/> request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="audioType">Type of the audio response</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public UniTask<AudioClip> AudioRequestAsync(AudioType audioType, CancellationToken cancellationToken = default)
        {
            return WebRequestUtility.AudioRequestAsync(AbsoluteUrl, audioType, SetRequestData, cancellationToken);
        }

        /// <summary>
        /// Make a detailed async <see cref="Texture2D"/> request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="readable">Texture response readable</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="WebResponse{TBody}"/></returns>
        public UniTask<IWebResponse<Texture2D>> DetailedTextureRequestAsync(bool readable, CancellationToken cancellationToken = default)
        {
            return WebRequestUtility.DetailedTextureRequestAsync(AbsoluteUrl, readable, SetRequestData, cancellationToken);
        }

        /// <summary>
        /// Make a detailed async <see cref="AudioClip"/> request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <param name="audioType">Type of the audio response</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="WebResponse{TBody}"/></returns>
        public UniTask<IWebResponse<AudioClip>> DetailedAudioRequestAsync(AudioType audioType, CancellationToken cancellationToken = default)
        {
            return WebRequestUtility.DetailedAudioRequestAsync(AbsoluteUrl, audioType, SetRequestData, cancellationToken);
        }
    }
}
#endif
