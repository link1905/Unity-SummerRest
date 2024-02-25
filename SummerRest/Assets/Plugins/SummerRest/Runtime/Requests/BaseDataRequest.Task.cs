#if SUMMER_REST_TASK
using System.Threading;
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
        public UniTask<TResponse> DataRequestAsync<TResponse>(CancellationToken cancellationToken = default)
        {
            return WebRequestUtility.DataRequestAsync<TResponse>(AbsoluteUrl, Method, 
                SerializedBody, ContentType, SetRequestData, cancellationToken);
        }    
        /// <summary>
        /// Make an async data request <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns><see cref="WebResponse{TBody}"/></returns>
        public UniTask<IWebResponse<TResponse>> DetailedDataRequestAsync<TResponse>(CancellationToken cancellationToken = default)
        {
            return WebRequestUtility.DetailedDataRequestAsync<TResponse>(AbsoluteUrl, Method, 
                SerializedBody, ContentType, SetRequestData, cancellationToken);
        }       
    }
}
#endif