using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.RequestAdaptor;

namespace Auth
{
    public class DummyJsonApiAuthAppender : IAuthAppender<DummyJsonApiAuthAppender, string>
    {
        private const string AuthKeyword = "Bearer";
        public void Append<TResponse>(string data, IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            // Append a header "Authorization: Bearer <my-token>"  
            requestAdaptor.SetHeader("Authorization", $"{AuthKeyword} {data}");
        }
    }
}