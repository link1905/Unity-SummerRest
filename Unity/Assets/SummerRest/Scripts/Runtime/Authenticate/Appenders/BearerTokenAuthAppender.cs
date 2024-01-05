using SummerRest.Runtime.RequestAdaptor;

namespace SummerRest.Runtime.Authenticate.Appenders
{
    public class BearerTokenAuthAppender : IAuthAppender<BearerTokenAuthAppender>
    {
        public void Append<TResponse>(IWebRequestAdaptor<TResponse> requestAdaptor)
        {
        }
    }
}