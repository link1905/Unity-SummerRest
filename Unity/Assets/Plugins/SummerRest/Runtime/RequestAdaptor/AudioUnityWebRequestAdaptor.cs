using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    internal class AudioUnityWebRequestAdaptor : UnityWebRequestAdaptor<AudioUnityWebRequestAdaptor, AudioClip>
    {

        internal override AudioClip BuildResponse()
        {
            return DownloadHandlerAudioClip.GetContent(WebRequest);
        }
    }
}