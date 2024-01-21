using SummerRest.Runtime.Authenticate.TokenRepositories;
using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestAdaptor;

namespace SummerRest.Runtime.Authenticate.Appenders
{
    public interface IAuthAppender
    {
        void Append<TResponse>(string authDataKey, IWebRequestAdaptor<TResponse> requestAdaptor);
    }

    /// <summary>
    /// Inherit this interface (please do not directly use <see cref="IAuthAppender"/>) to make your own appender <br/>
    /// The responsibility of an appender typically append values into the header of a request for validating processes on server side <br/>
    /// You should query <see cref="IAuthDataRepository"/> (or make your own system) to get auth values (token, password...) because of keeping login sessions and securing the secret values 
    /// </summary>
    /// <typeparam name="TAuthAppender">Type of your appender</typeparam>
    public interface IAuthAppender<TAuthAppender> : IAuthAppender, ISingleton<TAuthAppender>
        where TAuthAppender : class, ISingleton<TAuthAppender>, new()
    {
        
    }
}