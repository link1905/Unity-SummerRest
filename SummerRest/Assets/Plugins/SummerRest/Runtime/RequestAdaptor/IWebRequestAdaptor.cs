using System;
using System.Collections;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    /// <summary>
    /// Adaptor interface to abstract and modify metrics of the underlying requester <br/>
    /// Default adaptor is <see cref="UnityWebRequestAdaptor{TSelf,TResponse}"/> wrapping <see cref="UnityWebRequest"/>. Users can write their own adaptor (eg. DotnetHttpClient,...) by inheriting <see cref="IWebRequestAdaptor{TResponse}"/> and <see cref="IWebRequestAdaptorProvider"/>
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IWebRequestAdaptor<TResponse> : IDisposable
    {
        string Url { get; set; }
        void SetHeader(string key, string value);
        bool IsError(out ResponseError error);
        string GetHeader(string key);
        HttpMethod Method { get; set; }
        int RedirectLimit { get; set; }
        int TimeoutSeconds { get; set; }
        ContentType? ContentType { get; set; }
        IEnumerator RequestInstruction { get; }
        IWebResponse<TResponse> WebResponse { get; }
        TResponse ResponseData { get; }
    }
}