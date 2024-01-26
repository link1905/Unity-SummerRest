using SummerRest.Runtime.Authenticate.TokenRepositories;
using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestAdaptor;

namespace SummerRest.Runtime.Authenticate.Appenders
{
    /// <summary>
    /// Inherit this interface to make your own appender <br/>
    /// The responsibility of an appender typically append values into the header of a request for validating processes on server side <br/>
    /// You should query <see cref="IAuthDataRepository"/> (or make your own system) to get auth values (token, password...) because of keeping login sessions and securing the secret values 
    /// </summary>
    /// <typeparam name="TAuthAppender">Type of your appender</typeparam>
    /// <typeparam name="TAuthData">Type of your auth value</typeparam>
    public interface IAuthAppender<TAuthAppender, TAuthData> : ISingleton<TAuthAppender>
        where TAuthAppender : class, ISingleton<TAuthAppender>, new()
    {
        void Append<TResponse>(TAuthData authData, IWebRequestAdaptor<TResponse> requestAdaptor);
    }
}