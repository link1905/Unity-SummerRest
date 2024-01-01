using System;
using System.Collections;
using System.Threading.Tasks;
using SummerRest.Runtime.Request;
using SummerRest.Scripts.Utilities.RequestComponents;

namespace SummerRest.Runtime.RequestAdaptor
{
    internal interface IWebRequestAdaptor<TResponse> : IDisposable
    {
        string Url { get; set; }
        void SetHeader(string key, string value);
        HttpMethod Method { get; set; }
        int RedirectLimit { get; set; }
        int TimeoutSeconds { get; set; }
        ContentType ContentType { get; set; }
        IEnumerator RequestInstruction { get; }
        IWebResponse<TResponse> WebResponse { get; }
        TResponse ResponseData { get; }
        Task<TResponse> RequestAsync { get; }
    }
}