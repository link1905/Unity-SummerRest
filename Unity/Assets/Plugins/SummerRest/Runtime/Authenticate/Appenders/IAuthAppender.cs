using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestAdaptor;

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