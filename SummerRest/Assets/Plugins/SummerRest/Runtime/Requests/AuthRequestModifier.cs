using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Authenticate.TokenRepositories;
using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Runtime.Requests
{
    /// <summary>
    /// A modification of the auth method rarely happens, so we constraint it with the selected type in the plugin window 
    /// </summary>
    /// <typeparam name="TAuthAppender">Type of the selected appender in the plugin window</typeparam>
    /// <typeparam name="TAuthData">Type of auth data</typeparam>
    public class AuthRequestModifier<TAuthAppender, TAuthData> : IRequestModifier
        where TAuthAppender : class, IAuthAppender<TAuthAppender, TAuthData>, new()
    {
        /// <summary>
        /// Used to resolve auth values in <see cref="IAuthDataRepository"/> <seealso cref="IAuthData"/> <seealso cref="IAuthAppender{TAuthAppender, TAuthData}"/> <br/>
        /// </summary>
        public string AuthKey { get; set; }
        public void ModifyRequestData<TResponse>(IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            if (string.IsNullOrEmpty(AuthKey))
                return;
            var appender = ISingleton<TAuthAppender>.GetSingleton();
            if (!IAuthDataRepository.Current.TryGet<TAuthData>(AuthKey, out var authData))
            {
                Debug.LogWarningFormat(@"The auth key ""{0}"" does not exist in the program", AuthKey);
                return;
            }
            appender.Append(authData, requestAdaptor);
        }
    }
}