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
        public IWebRequestAdaptor<Texture2D> GetTextureRequest(string url, bool nonReadable)
        {
            var request = UnityWebRequestTexture.GetTexture(url, nonReadable);
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
            string url, HttpMethod method, string bodyData, ContentType? contentType)
        {
            UnityWebRequest request;
            switch (method)
            {
                case HttpMethod.Get or HttpMethod.Trace or HttpMethod.Connect or HttpMethod.Options:
                    request = UnityWebRequest.Get(url);
                    break;
                case HttpMethod.Post:
                    var textContentType = contentType?.FormedContentType;
                    if (string.IsNullOrEmpty(textContentType))
                        textContentType = ContentType.Commons.TextPlain.FormedContentType;
                    request = UnityWebRequest.Post(url, bodyData, textContentType);
                    break;
                case HttpMethod.Put  or HttpMethod.Patch:
                    request = UnityWebRequest.Put(url, bodyData);
                    break;
                case HttpMethod.Delete:
                    request = UnityWebRequest.Delete(url);
                    break;
                default:
                    request = new UnityWebRequest(url, method.ToUnityHttpMethod());
                    break;
            }

            request.method = method.ToUnityHttpMethod();
            return DataUnityWebRequestAdaptor<TResponse>.Create(request);
        }

        public IWebRequestAdaptor<TResponse> GetMultipartFileRequest<TResponse>(string url,
            HttpMethod method,
            List<IMultipartFormSection> data, ContentType? contentType)
        {
            var request = contentType is null ? UnityWebRequest.Post(url, data) : UnityWebRequest.Post(url, data, contentType.Value.BoundaryBytes);
            request.method = method.ToUnityHttpMethod();
            return MultipartFileUnityWebRequestAdaptor<TResponse>.Create(request);
        }
    }
}