using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    internal class TextureUnityWebRequestAdaptor : UnityWebRequestAdaptor<TextureUnityWebRequestAdaptor, Texture2D>
    {
        internal override Texture2D BuildResponse()
        {
            return DownloadHandlerTexture.GetContent(WebRequest);
        }
    }
}