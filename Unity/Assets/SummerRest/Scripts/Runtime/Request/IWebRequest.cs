using System.Collections.Generic;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Utilities.RequestComponents;

namespace SummerRest.Runtime.Request
{
    public interface IWebRequest
    {
        string Url { get; set; }
        IDictionary<string, string> Headers { get; }
        RequestParamContainer Params { get; }
        HttpMethod Method { get; set; }
        int? RedirectLimit { get; set; }
        int? TimeoutSeconds { get; set; }
        string AuthKey { get; set; }
        IAuthAppender AuthAppender { get; set; }
        // IAuthData AuthData { get; set; }
        ContentType? ContentType { get; set; }
    }

    public interface IWebRequest<TBody> : IWebRequest
    {
        TBody BodyData { get; set; }
        DataFormat BodyFormat { get; set; }
    }
}