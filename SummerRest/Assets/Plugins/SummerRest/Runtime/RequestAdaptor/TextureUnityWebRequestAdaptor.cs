using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    /// <summary>
    /// Request adaptor getting the <see cref="Texture2D"/> result of a UnityWebRequest
    /// </summary>
    internal class TextureUnityWebRequestAdaptor : UnityWebRequestAdaptor<TextureUnityWebRequestAdaptor, Texture2D>
    {
        internal override Texture2D BuildResponse()
        {
            return DownloadHandlerTexture.GetContent(WebRequest);
        }
    }
}