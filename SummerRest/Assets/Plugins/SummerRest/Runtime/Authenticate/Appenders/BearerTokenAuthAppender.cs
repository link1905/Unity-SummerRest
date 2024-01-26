using SummerRest.Runtime.RequestAdaptor;
using UnityEngine;

namespace SummerRest.Runtime.Authenticate.Appenders
{
    /// <summary>
    /// Bearer token appender simply adding a header {"Authorization": "Bearer ..."}
    /// </summary>
    public class BearerTokenAuthAppender : IAuthAppender<BearerTokenAuthAppender, string>
    {
        private const string AuthKeyword = "Bearer";
        public void Append<TResponse>(string data, IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            if (string.IsNullOrEmpty(data))
            {
                Debug.LogWarningFormat("Bearer token is null or empty");
                return;
            }
            requestAdaptor.SetHeader("Authorization", $"{AuthKeyword} {data}");
        }
    }
}