using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;
using UnityEngine.Networking;

namespace SummerRest.Runtime.Requests
{
    public abstract class BaseMultipartRequest<TRequest> : BaseRequest<TRequest> where TRequest : BaseRequest<TRequest>, new()
    {
        /// <summary>
        /// The form data of arisen requests <br/>
        /// Originally, this property only reflects on text fields (you must insert file sections manually) <seealso cref="MultipartFormDataSection"/> <see cref="MultipartFormFileSection"/> <br/>
        /// </summary>
        public readonly List<IMultipartFormSection> MultipartFormSections = new();
        public BaseMultipartRequest(string url, string absoluteUrl, IRequestModifier requestModifier) : 
            base(url, absoluteUrl, requestModifier)
        {
        }
        /// <summary>
        /// Simple data request that uploads multipart file sections (the boundary leverages <see cref="ContentType"/>)
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <returns></returns>
        public IEnumerator RequestCoroutine<TResponse>(Action<TResponse> doneCallback,
            Action<ResponseError> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetMultipartFileRequest<TResponse>(AbsoluteUrl, MultipartFormSections);
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }
        /// <summary>
        /// Detailed data request that uploads multipart file sections (the boundary leverages <see cref="ContentType"/>)
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <returns></returns>
        public IEnumerator DetailedRequestCoroutine<TResponse>(Action<WebResponse<TResponse>> doneCallback,
            Action<ResponseError> errorCallback = null)
        {
            using var request = IWebRequestAdaptorProvider.Current.GetMultipartFileRequest<TResponse>(AbsoluteUrl, MultipartFormSections);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }
        
        /// <summary>
        /// Make an async data request that uploads multipart file sections. Please note that this method throws an exception when encountering issues
        /// </summary>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns></returns>
        public UniTask<TResponse> RequestAsync<TResponse>()
        {
            using var request = IWebRequestAdaptorProvider.Current.GetMultipartFileRequest<TResponse>(AbsoluteUrl, MultipartFormSections);
            return RequestAsync(request);
        }    
        /// <summary>
        /// Make a detailed async data request that uploads multipart file sections
        /// </summary>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns>A response object contains HTTP response's essential fields</returns>
        public UniTask<WebResponse<TResponse>> DetailedRequestAsync<TResponse>()
        {
            using var request = IWebRequestAdaptorProvider.Current.GetMultipartFileRequest<TResponse>(AbsoluteUrl, MultipartFormSections);
            return DetailedRequestAsync(request);
        }       
    }
}