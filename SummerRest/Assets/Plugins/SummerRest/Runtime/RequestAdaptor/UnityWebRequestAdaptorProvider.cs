using System;
using System.Collections.Generic;
using System.Text;
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
            string url, HttpMethod method, string bodyData, string contentType)
        {
            UnityWebRequest request;
            switch (method)
            {
                case HttpMethod.Get or HttpMethod.Trace or HttpMethod.Connect or HttpMethod.Options:
                    request = UnityWebRequest.Get(url);
                    break;
                case HttpMethod.Post:
                    var textContentType = contentType;
                    if (string.IsNullOrEmpty(textContentType))
                        textContentType = ContentType.Commons.ApplicationJson.FormedContentType;
                    request = PostFromStringData(url, bodyData, textContentType);
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
            return DataUnityWebRequestAdaptor<TResponse>.Create(request);
        }
        private static UnityWebRequest PostFromStringData(string uri, string postData, string contentType)
        {
            var request = new UnityWebRequest(uri, "POST");
            var bytes = Encoding.UTF8.GetBytes(postData);
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.uploadHandler.contentType = contentType;
            return request;
        }
        public IWebRequestAdaptor<TResponse> GetMultipartFileRequest<TResponse>(string url,
            List<IMultipartFormSection> data, byte[] boundary)
        {
            if (data is { Count: 0 })
                throw new ArgumentException(@$"Multipart form body of the resource ""{url}"" is empty");
            var request = PostMultipartFormData(url, data, boundary);
            return MultipartFileUnityWebRequestAdaptor<TResponse>.Create(request);
        }
        
        private static UnityWebRequest PostMultipartFormData(
            string url,
            List<IMultipartFormSection> multipartFormSections,
            byte[] boundary)
        {
            var request = new UnityWebRequest(url);
            boundary ??= UnityWebRequest.GenerateBoundary();
            request.downloadHandler = new DownloadHandlerBuffer();
            var data = UnityWebRequest.SerializeFormSections(multipartFormSections, boundary);
            UploadHandler uploadHandler = new UploadHandlerRaw(data);
            request.uploadHandler = uploadHandler;
            return request;
        }
    }
}