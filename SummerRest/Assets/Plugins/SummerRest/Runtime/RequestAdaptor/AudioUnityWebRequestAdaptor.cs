using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    /// <summary>
    /// Request adaptor getting the <see cref="AudioClip"/> result of a UnityWebRequest
    /// </summary>
    internal class AudioUnityWebRequestAdaptor : UnityWebRequestAdaptor<AudioUnityWebRequestAdaptor, AudioClip>
    {

        internal override AudioClip BuildResponse()
        {
            return DownloadHandlerAudioClip.GetContent(WebRequest);
        }
    }
}