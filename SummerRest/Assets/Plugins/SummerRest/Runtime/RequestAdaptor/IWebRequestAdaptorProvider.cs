using System.Collections.Generic;
using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    /// <summary>
    /// Providing adaptors for calling HTTP endpoints <br/>
    /// Default support is <see cref="UnityWebRequestAdaptorProvider"/> providing <see cref="UnityWebRequestAdaptor{TSelf,TResponse}"/> <br/>
    /// You can adapt <see cref="IDefaultSupport{TInterface,TDefault}.Current"/> to your preference in runtime
    /// </summary>
    public interface IWebRequestAdaptorProvider : IDefaultSupport<IWebRequestAdaptorProvider, UnityWebRequestAdaptorProvider>
    {
        IWebRequestAdaptor<Texture2D> GetTextureRequest(string url, bool nonReadable);
        IWebRequestAdaptor<AudioClip> GetAudioRequest(string url, AudioType audioType);
        IWebRequestAdaptor<TBody> GetDataRequest<TBody>(string url, HttpMethod method, string bodyData, ContentType? contentType);
        IWebRequestAdaptor<TBody> GetMultipartFileRequest<TBody>(string url, HttpMethod method, List<IMultipartFormSection> data, ContentType? contentType);
    }
}