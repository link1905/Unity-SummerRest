using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.Requests
{
    
    public abstract class BaseDataRequest<TRequest> : BaseRequest<TRequest> where TRequest : BaseRequest<TRequest>, new() 
    {
        public BaseDataRequest(string url, string absoluteUrl, IRequestModifier requestModifier) : base(url, absoluteUrl, requestModifier)
        {
            
        }
        private object _bodyData;
        /// <summary>
        /// The body data of arisen requests <br/>
        /// Originally, this property is null since we can not refer to your custom class for deserializing <br/>
        /// This property is only useful if <see cref="BaseRequest{TRequest}.Method"/> is {<see cref="HttpMethod.Post"/>, <see cref="HttpMethod.Put"/>, <see cref="HttpMethod.Patch"/>}
        /// </summary>
        public object BodyData { get => _bodyData;
            set
            {
                // Do not leverage the initialized json string anymore after users set something new
                InitializedSerializedBody = null;
                _bodyData = value;
            }
        }
        /// <summary>
        /// The body format of arisen requests
        /// </summary>
        public DataFormat BodyFormat { get; set; }
        protected string InitializedSerializedBody { get; set; }
        /// <summary>
        /// It may be better if we cache this value when <see cref="BodyData"/> or <see cref="BodyFormat"/> are changed <br/>
        /// But the problem comes up if users change the <see cref="BodyData"/> properties outside this class, then the cache is wrong
        /// </summary>
        internal virtual string SerializedBody => 
            //InitializedSerializedBody is null => use the body instead
            !string.IsNullOrEmpty(InitializedSerializedBody) ? 
                InitializedSerializedBody : BodyData is null ? 
                    null : IDataSerializer.Current.Serialize(BodyData, BodyFormat);
        /// <summary>
        /// Simple data request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns></returns>
        public IEnumerator RequestCoroutine<TResponse>(Action<TResponse> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody, ContentType?.FormedContentType);
            yield return RequestCoroutine(request, doneCallback, errorCallback);
        }
        /// <summary>
        /// Detailed data request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        public IEnumerator DetailedRequestCoroutine<TResponse>(Action<WebResponse<TResponse>> doneCallback,
            Action<string> errorCallback = null)
        {
            using var request =
                IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody, ContentType?.FormedContentType);
            yield return DetailedRequestCoroutine(request, doneCallback, errorCallback);
        }
        
        /// <summary>
        /// Make an async data request. Please note that this method throws an exception when encountering issues
        /// </summary>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns></returns>
        public UniTask<TResponse> RequestAsync<TResponse>()
        {
            using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody, ContentType?.FormedContentType);
            return RequestAsync(request);
        }    
        /// <summary>
        /// Make a detailed async data request
        /// </summary>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns>A response object contains HTTP response's essential fields</returns>
        public UniTask<WebResponse<TResponse>> DetailedRequestAsync<TResponse>()
        {
            using var request = IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody, ContentType?.FormedContentType);
            return DetailedRequestAsync(request);
        }       
    }
}