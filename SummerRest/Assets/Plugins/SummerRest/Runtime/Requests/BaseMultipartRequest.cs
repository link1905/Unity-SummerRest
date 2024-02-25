using System;
using System.Collections;
using System.Collections.Generic;
using SummerRest.Runtime.RequestAdaptor;
using UnityEngine.Networking;

namespace SummerRest.Runtime.Requests
{
    public abstract partial class BaseMultipartRequest<TRequest> : BaseRequest<TRequest> where TRequest : BaseRequest<TRequest>, new()
    {
        /// <summary>
        /// The form data of arisen requests <br/>
        /// Originally, this property only reflects on text fields (you must insert file sections manually) <seealso cref="MultipartFormDataSection"/> <see cref="MultipartFormFileSection"/> <br/>
        /// </summary>
        public readonly List<IMultipartFormSection> MultipartFormSections = new();
        public BaseMultipartRequest(string url, string absoluteUrl,  string urlFormat, string[] urlFormatValues, IRequestModifier requestModifier) : 
            base(url, absoluteUrl, urlFormat, urlFormatValues, requestModifier)
        {
        }
        /// <summary>
        /// Simple data request that uploads multipart file sections
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <returns></returns>
        public IEnumerator MultipartDataRequestCoroutine<TResponse>(Action<TResponse> doneCallback,
            Action<ResponseError> errorCallback = null)
        {
            return WebRequestUtility.MultipartDataRequestCoroutine(AbsoluteUrl, Method, 
                MultipartFormSections, ContentType, doneCallback, errorCallback, SetRequestData);
        }
        /// <summary>
        /// Detailed data request that uploads multipart file sections
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <returns></returns>
        /// <remarks>Please remember to dispose <see cref="IWebResponse{TBody}"/> for avoiding memory leakage</remarks>
        public IEnumerator DetailedMultipartDataRequestCoroutine<TResponse>(Action<IWebResponse<TResponse>> doneCallback,
            Action<ResponseError> errorCallback = null)
        {
            return WebRequestUtility.DetailedMultipartDataRequestCoroutine(AbsoluteUrl, Method, 
                MultipartFormSections, ContentType, doneCallback, errorCallback, SetRequestData);
        }

    }
}