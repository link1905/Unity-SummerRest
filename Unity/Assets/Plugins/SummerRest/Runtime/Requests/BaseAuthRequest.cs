using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Authenticate.TokenRepositories;
using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.Requests
{
    /// <summary>
    /// Represents a request that need to be authenticated <br/>
    /// A modification of the auth method rarely happens, so we constraint it with the selected type in the plugin window 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TAuthAppender">Type of the selected appender in the plugin window</typeparam>
    public abstract class BaseAuthRequest<TRequest, TAuthAppender> : BaseRequest<TRequest>
        where TRequest : BaseAuthRequest<TRequest, TAuthAppender>, new()          
        where TAuthAppender : class, IAuthAppender<TAuthAppender>, new()
    {
        /// <summary>
        /// Used to resolve auth values in <see cref="IAuthDataRepository"/> <seealso cref="IAuthData"/> <seealso cref="IAuthAppender{TAuthAppender}"/> <br/>
        /// </summary>
        public string AuthKey { get; set; }
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