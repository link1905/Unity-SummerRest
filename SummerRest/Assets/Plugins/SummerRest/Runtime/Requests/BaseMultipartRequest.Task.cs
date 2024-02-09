#if SUMMER_REST_TASK
using Cysharp.Threading.Tasks;
using SummerRest.Runtime.RequestAdaptor;

namespace SummerRest.Runtime.Requests
{
    public partial class BaseMultipartRequest<TRequest>
    {
        /// <summary>
        /// Make an async data request that uploads multipart file sections <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns></returns>
        public UniTask<TResponse> MultipartDataRequestAsync<TResponse>()
        {
            return WebRequestUtility.MultipartDataRequestAsync<TResponse>(AbsoluteUrl, Method, MultipartFormSections, SetRequestData);
        }    
        /// <summary>
        /// Make an async data request that uploads multipart file sections <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns><see cref="WebResponse{TBody}"/></returns>
        public UniTask<WebResponse<TResponse>> DetailedMultipartDataRequestAsync<TResponse>()
        {
            return WebRequestUtility.DetailedMultipartDataRequestAsync<TResponse>(AbsoluteUrl, Method, MultipartFormSections, SetRequestData);
        }       
    }
}
#endif