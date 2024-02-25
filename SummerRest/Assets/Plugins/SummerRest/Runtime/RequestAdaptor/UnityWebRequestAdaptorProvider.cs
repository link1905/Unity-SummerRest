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
            var request = new UnityWebRequest(url, 
                method.ToUnityHttpMethod(), 
                new DownloadHandlerBuffer(), 
                null);
            if (!string.IsNullOrEmpty(bodyData))
            {
                var textContentType = contentType;
                if (string.IsNullOrEmpty(textContentType))
                    textContentType = ContentType.Commons.ApplicationJson.FormedContentType;
                AppendUploadStringData(request, bodyData, textContentType);
            }
            return DataUnityWebRequestAdaptor<TResponse>.Create(request);
        }
        private static void AppendUploadStringData(UnityWebRequest webRequest, 
            string data, string contentType)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            webRequest.uploadHandler = new UploadHandlerRaw(bytes);
            webRequest.uploadHandler.contentType = contentType;
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