using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Authenticate.TokenRepositories;
using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestAdaptor;
using UnityEngine;

namespace SummerRest.Runtime.Requests
{
    /// <summary>
    /// A modification of the auth method rarely happens, so we constraint it with the selected type in the plugin window 
    /// </summary>
    /// <typeparam name="TAuthAppender">Type of the selected appender in the plugin window</typeparam>
    /// <typeparam name="TAuthData">Type of auth data</typeparam>
    internal class AuthRequestModifier<TAuthAppender, TAuthData> : IRequestModifier<AuthRequestModifier<TAuthAppender, TAuthData>> 
        where TAuthAppender : class, IAuthAppender<TAuthAppender, TAuthData>, new()
    {

        /// <summary>
        /// Resolve then add auth values into the request adaptor 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="requestAdaptor"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        public void ModifyRequestData<TRequest, TResponse>(BaseRequest<TRequest> request, IWebRequestAdaptor<TResponse> requestAdaptor) 
            where TRequest : BaseRequest<TRequest>, new()
        {
            var authKey = request.AuthKey;
            if (string.IsNullOrEmpty(authKey))
                return;
            var appender = ISingleton<TAuthAppender>.GetSingleton();
            if (!IAuthDataRepository.Current.TryGet<TAuthData>(authKey, out var authData))
            {
                Debug.LogWarningFormat(@"The auth key ""{0}"" does not exist in the program", authKey);
                return;
            }
            appender.Append(authData, requestAdaptor);
        }
    }
}