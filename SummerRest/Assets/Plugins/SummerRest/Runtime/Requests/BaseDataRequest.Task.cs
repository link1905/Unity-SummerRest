#if SUMMER_REST_TASK
using Cysharp.Threading.Tasks;
using SummerRest.Runtime.RequestAdaptor;

namespace SummerRest.Runtime.Requests
{
    public partial class BaseDataRequest<TRequest>
    {
                
        /// <summary>
        /// Make an async data request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns></returns>
        public UniTask<TResponse> DataRequestAsync<TResponse>()
        {
            return WebRequestUtility.DataRequestAsync<TResponse>(AbsoluteUrl, Method, 
                SerializedBody, ContentType, SetRequestData);
        }    
        /// <summary>
        /// Make an async data request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns><see cref="WebResponse{TBody}"/></returns>
        public UniTask<WebResponse<TResponse>> DetailedDataRequestAsync<TResponse>()
        {
            return WebRequestUtility.DetailedDataRequestAsync<TResponse>(AbsoluteUrl, Method, 
                SerializedBody, ContentType, SetRequestData);
        }       
    }
}
#endif