using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    public class AudioUnityWebRequestAdaptor : UnityWebRequestAdaptor<AudioUnityWebRequestAdaptor, AudioClip>
    {
        protected override void DoneRequest()
        {
            ResponseData = DownloadHandlerAudioClip.GetContent(WebRequest);
        }
    }
}