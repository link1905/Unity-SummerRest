using System;
using System.Collections;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.Requests
{
    
    
    
    public abstract partial class BaseDataRequest<TRequest> : BaseRequest<TRequest> where TRequest : BaseRequest<TRequest>, new() 
    {
        public BaseDataRequest(string url, string absoluteUrl,  string urlFormat, string[] urlFormatValues, IRequestModifier requestModifier) : 
            base(url, absoluteUrl, urlFormat, urlFormatValues, requestModifier)
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
        /// <summary>
        /// Leverage this value till users set their own value
        /// </summary>
        protected string InitializedSerializedBody { get; set; }
        /// <summary>
        /// It may be better if we cache this value when <see cref="BodyData"/> or <see cref="BodyFormat"/> are changed <br/>
        /// But the problem comes up if users change the <see cref="BodyData"/> properties outside this class, then the cache is wrong
        /// </summary>
        internal string SerializedBody => 
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
        public IEnumerator DataRequestCoroutine<TResponse>(Action<TResponse> doneCallback,
            Action<ResponseError> errorCallback = null)
        {
            return WebRequestUtility.DataRequestCoroutine(AbsoluteUrl, Method,
                doneCallback, SerializedBody, ContentType, errorCallback, SetRequestData);
        }
        /// <summary>
        /// Detailed data request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <typeparam name="TResponse">Type which the response data will be deserialized into</typeparam>
        /// <returns></returns>
        public IEnumerator DetailedRequestCoroutine<TResponse>(Action<WebResponse<TResponse>> doneCallback,
            Action<ResponseError> errorCallback = null)
        {
            return WebRequestUtility.DetailedDataRequestCoroutine(AbsoluteUrl, Method,
                doneCallback, SerializedBody, ContentType, errorCallback, SetRequestData);
        }

    }
}