using System.Collections.Generic;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.Request
{
    public interface IWebRequest
    {
        string Url { get; set; }
        IDictionary<string, string> Headers { get; }
        RequestParamContainer Params { get; }
        HttpMethod Method { get; set; }
        int? RedirectsLimit { get; set; }
        int? TimeoutSeconds { get; set; }
        string AuthKey { get; set; }
        // IAuthData AuthData { get; set; }
        ContentType? ContentType { get; set; }
    }

    public interface IWebRequest<TBody> : IWebRequest
    {
        TBody BodyData { get; set; }
        DataFormat BodyFormat { get; set; }
    }
}