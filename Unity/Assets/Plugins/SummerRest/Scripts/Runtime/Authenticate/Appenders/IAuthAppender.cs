using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Utilities.DataStructures;

namespace SummerRest.Runtime.Authenticate.Appenders
{
    public interface IAuthAppender
    {
        void Append<TResponse>(string authDataKey, IWebRequestAdaptor<TResponse> requestAdaptor);
    }

    public interface IAuthAppender<TAuthAppender> : IAuthAppender, ISingleton<TAuthAppender>
        where TAuthAppender : class, ISingleton<TAuthAppender>, new()
    {
        
    }
}