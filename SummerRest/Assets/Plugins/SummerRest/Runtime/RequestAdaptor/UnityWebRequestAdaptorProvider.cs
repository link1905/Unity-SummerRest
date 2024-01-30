using System;
using System.Collections.Generic;
using SummerRest.Runtime.Extensions;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{

    /// <summary>
    /// Wrapping <see cref="UnityWebRequest"/> static factory methods to create <see cref="UnityWebRequestAdaptor{TSelf,TResponse}"/>
    /// </summary>
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
            string url, HttpMethod method, string bodyData, string contentType)
        {
            UnityWebRequest request;
            switch (method)
            {
                case HttpMethod.Get or HttpMethod.Trace or HttpMethod.Connect or HttpMethod.Options:
                    request = UnityWebRequest.Get(url);
                    break;
                case HttpMethod.Post or HttpMethod.Patch:
                    request = UnityWebRequest.Post(url, bodyData, contentType);
                    break;
                case HttpMethod.Put:
                    request = UnityWebRequest.Put(url, bodyData);
                    break;
                case HttpMethod.Delete:
                    request = UnityWebRequest.Delete(url);
                    break;
                default:
                    request = new UnityWebRequest(url, method.ToUnityHttpMethod());
                    break;
            }
            return RawUnityWebRequestAdaptor<TResponse>.Create(request);
        }

        public IWebRequestAdaptor<TResponse> GetMultipartFileRequest<TResponse>(string url, List<IMultipartFormSection> data)
        {
            var request = UnityWebRequest.Post(url, data, Array.Empty<byte>());
            return MultipartFileUnityWebRequestAdaptor<TResponse>.Create(request);
        }
    }
}