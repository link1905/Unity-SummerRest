using SummerRest.Runtime.Extensions;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    public class UnityWebRequestAdaptorProvider : IWebRequestAdaptorProvider
    {
        public IWebRequestAdaptor<Texture2D> GetTextureRequest(string url, bool readable)
        {
            var request = UnityWebRequestTexture.GetTexture(url, readable);
            return TextureUnityWebRequestAdaptor.Create(request);
        }
        public IWebRequestAdaptor<AudioClip> GetAudioRequest(string url, AudioType audioType)
        {
            var request = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            return AudioUnityWebRequestAdaptor.Create(request);
        }
        public IWebRequestAdaptor<UnityWebRequest> GetFromUnityWebRequest(UnityWebRequest webRequest)
        {
            return DumpUnityWebRequestAdaptor.Create(webRequest);
        }
        public IWebRequestAdaptor<TResponse> GetDataRequest<TResponse>(
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
            return RawUnityWebRequestAdaptor<TResponse>.Create(request);
        }
    }
}