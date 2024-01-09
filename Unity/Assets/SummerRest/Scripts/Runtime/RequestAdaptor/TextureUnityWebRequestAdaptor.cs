using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    public class TextureUnityWebRequestAdaptor : UnityWebRequestAdaptor<TextureUnityWebRequestAdaptor, Texture2D>
    {
        public override string RawResponse => null;

        protected override void DoneRequest()
        {
            ResponseData = DownloadHandlerTexture.GetContent(WebRequest);
        }
    }
}