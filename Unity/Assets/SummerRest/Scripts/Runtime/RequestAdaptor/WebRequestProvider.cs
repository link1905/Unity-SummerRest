using SummerRest.Scripts.Utilities.Extensions;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    internal static class WebRequestProvider
    {
        internal static IWebRequestAdaptor<Texture2D> GetTextureRequest(string url, bool readable)
        {
            var request = UnityWebRequestTexture.GetTexture(url, readable);
            return UnityWebRequestAdaptor<Texture2D>.Create(request);
        }
        internal static IWebRequestAdaptor<AudioClip> GetAudioRequest(string url, AudioType audioType)
        {
            var request = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            return UnityWebRequestAdaptor<AudioClip>.Create(request);
        }
        internal static IWebRequestAdaptor<TResponse> GetFromUnityWebRequest<TResponse>(UnityWebRequest webRequest)
        {
            return UnityWebRequestAdaptor<TResponse>.Create(webRequest);
        }
        internal static IWebRequestAdaptor<TBody> GetDataRequest<TBody>(
            string url, HttpMethod method, string bodyData)
        {
            UnityWebRequest request;
            switch (method)
            {
                case HttpMethod.Get:
                    request = UnityWebRequest.Get(url);
                    break;
                case HttpMethod.Post:
                    request = UnityWebRequest.PostWwwForm(url, bodyData);
                    break;
                case HttpMethod.Put:
                    request = UnityWebRequest.Put(url, bodyData);
                    break;
                case HttpMethod.Delete:
                    request = UnityWebRequest.Delete(url);
                    break;
                case HttpMethod.Head:
                    request = UnityWebRequest.Head(url);
                    break;
                default:
                    request = new UnityWebRequest(url, method.ToUnityHttpMethod());
                    break;
            }
            return UnityWebRequestAdaptor<TBody>.Create(request);
        }
    }
}