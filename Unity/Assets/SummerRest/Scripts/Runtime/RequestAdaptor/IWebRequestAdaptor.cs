using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SummerRest.Runtime.Request;
using SummerRest.Scripts.Utilities.RequestComponents;

namespace SummerRest.Runtime.RequestAdaptor
{
    public interface IWebRequestAdaptor<TResponse> : IDisposable
    {
        string Url { get; set; }
        void SetHeader(string key, string value);
        bool IsError(out string error);
        string GetHeader(string key);
        HttpMethod Method { get; set; }
        int RedirectLimit { get; set; }
        int TimeoutSeconds { get; set; }
        ContentType? ContentType { get; set; }
        IEnumerator RequestInstruction { get; }
        WebResponse<TResponse> WebResponse { get; }
        TResponse ResponseData { get; }
    }
}