using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestAdaptor;

namespace SummerRest.Runtime.Requests
{
    public interface IRequestModifier<TRequestModifier> : ISingleton<TRequestModifier>, IRequestModifier
        where TRequestModifier : class, IRequestModifier<TRequestModifier>, new()
    {

    }
    public interface IRequestModifier
    {
        void ModifyRequestData<TRequest, TResponse>(BaseRequest<TRequest> request, IWebRequestAdaptor<TResponse> requestAdaptor) 
            where TRequest : BaseRequest<TRequest>, new();
    }
}