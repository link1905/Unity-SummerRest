using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Scripts.Utilities.DataStructures;

namespace SummerRest.Runtime.Authenticate.Appenders
{
    public interface IAuthAppender : 
        IDefaultSupport<IAuthAppender, BearerAuthAppender>
    {
        void Append<TResponse>(IWebRequestAdaptor<TResponse> requestAdaptor);
    }

    public interface IAuthAppender<TAuthAppender> : IAuthAppender, ISingleton<TAuthAppender>
        where TAuthAppender : class, new()
    {
    }
}