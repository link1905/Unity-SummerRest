using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Authenticate.TokenRepositories;
using SummerRest.Runtime.RequestAdaptor;

namespace SummerRestSample
{
    public class NinjaApiAuthAppender : IAuthAppender<NinjaApiAuthAppender, string>
    {
        public void Append<TResponse>(string authData, IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            requestAdaptor.SetHeader("X-Api-Key", authData);
        }
    }
}