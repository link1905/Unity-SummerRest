using System.Collections.Generic;
using System.Net;
using SummerRest.Scripts.Utilities.RequestComponents;

namespace SummerRest.Runtime.Request
{
    public interface IWebResponse<out TBody>
    {
        string RawData { get; }
        IEnumerable<KeyValuePair<string, string>> Headers { get; }
        HttpStatusCode StatusCode { get; }
        string Error { get; }
        ContentType ContentType { get; }
        TBody Data { get; }
    }
}