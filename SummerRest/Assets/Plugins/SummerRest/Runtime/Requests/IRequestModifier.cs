using SummerRest.Runtime.RequestAdaptor;

namespace SummerRest.Runtime.Requests
{
    public interface IRequestModifier
    {
        void ModifyRequestData<TResponse>(IWebRequestAdaptor<TResponse> requestAdaptor);
    }
}