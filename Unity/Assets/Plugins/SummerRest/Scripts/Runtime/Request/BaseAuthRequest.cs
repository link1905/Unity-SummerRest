using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Utilities.DataStructures;

namespace SummerRest.Runtime.Request
{
    public abstract class BaseAuthRequest<TRequest, TAuthAppender> : BaseRequest<TRequest>
        where TRequest : BaseAuthRequest<TRequest, TAuthAppender>, new()          
        where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
    {
        protected BaseAuthRequest(string url, string absoluteUrl) : base(url, absoluteUrl)
        {
            
        }
        protected override void SetRequestData<TResponse>(IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            base.SetRequestData(requestAdaptor);
            if (string.IsNullOrEmpty(AuthKey))
                return;
            var appender = ISingleton<TAuthAppender>.GetSingleton();
            appender.Append(AuthKey, requestAdaptor);
        }
    }
}