using SummerRest.Runtime.RequestAdaptor;

namespace SummerRest.Runtime.Authenticate.Appenders
{
    public class BearerAuthAppender : IAuthAppender<BearerAuthAppender>
    {
        public void Append<TResponse>(IWebRequestAdaptor<TResponse> requestAdaptor)
        {
        }
    }
}