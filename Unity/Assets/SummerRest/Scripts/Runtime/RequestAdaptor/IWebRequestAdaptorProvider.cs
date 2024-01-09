using SummerRest.Utilities.DataStructures;
using SummerRest.Utilities.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    public interface IWebRequestAdaptorProvider : IDefaultSupport<IWebRequestAdaptorProvider, UnityWebRequestAdaptorProvider>
    {
        IWebRequestAdaptor<Texture2D> GetTextureRequest(string url, bool readable);
        IWebRequestAdaptor<AudioClip> GetAudioRequest(string url, AudioType audioType);
        IWebRequestAdaptor<TResponse> GetFromUnityWebRequest<TResponse>(UnityWebRequest webRequest);
        IWebRequestAdaptor<TBody> GetDataRequest<TBody>(string url, HttpMethod method, string bodyData);
    }
}