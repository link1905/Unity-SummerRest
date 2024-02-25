#if SUMMER_REST_TASK
using System.Threading;
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
        public UniTask<TResponse> MultipartDataRequestAsync<TResponse>(CancellationToken cancellationToken = default)
        {
            return WebRequestUtility.MultipartDataRequestAsync<TResponse>(AbsoluteUrl, Method, MultipartFormSections, ContentType, SetRequestData, cancellationToken);
        }    
        /// <summary>
        /// Make an async data request that uploads multipart file sections <br/>
        /// Please note that this method throws an <see cref="ResponseErrorException"/> exception when encountering issues
        /// </summary>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns><see cref="WebResponse{TBody}"/></returns>
        public UniTask<IWebResponse<TResponse>> DetailedMultipartDataRequestAsync<TResponse>(CancellationToken cancellationToken = default)
        {
            return WebRequestUtility.DetailedMultipartDataRequestAsync<TResponse>(AbsoluteUrl, Method, MultipartFormSections, ContentType, SetRequestData, cancellationToken);
        }       
    }
}
#endif