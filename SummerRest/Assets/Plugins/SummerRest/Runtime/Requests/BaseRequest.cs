using System.Collections;
using System.Collections.Generic;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;
using UnityEngine.Networking;

namespace SummerRest.Runtime.Requests
{
    /// <summary>
    /// Base request which creates temporary requesters based on its properties <br/>
    /// This class' instances are reusable, do not create them too much to avoid heap allocation <br/>
    /// The properties are stable with the request's lifetime; That means whenever you modify a property, it will affect every sequent requests <br/>
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public abstract partial class BaseRequest<TRequest> where TRequest : BaseRequest<TRequest>, new() 
    {
        /// <summary>
        /// Absolute URL built from <see cref="Url"/> and <see cref="Params"/> 
        /// </summary>
        public string AbsoluteUrl { get; protected set; }
        private string _url;

        /// <summary>
        /// Original value is your input in the plugin window
        /// </summary>
        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                RebuildUrl();
            }
        }

        /// <summary>
        /// Access this to modify your params in runtime<br/>
        /// Please note that, this property originally contains your inputs in the plugin window
        /// </summary>
        public RequestParamContainer Params { get; } = new();

        /// <summary>
        /// Access this to modify your headers in runtime<br/>
        /// Please note that, this property originally contains your inputs in the plugin window
        /// </summary>
        public IDictionary<string, string> Headers { get; } =
            new Dictionary<string, string>();

        /// <summary>
        /// The method of arisen requests <br/>
        /// Please note that, this property only works with data request; texture, audio... requests automatically use <see cref="HttpMethod.Get"/>
        /// </summary>
        public HttpMethod Method { get; set; }
        /// <summary>
        /// The maximum redirects of arisen requests
        /// </summary>
        public int? RedirectsLimit { get; set; }
        /// <summary>
        /// The maximum timeout (in seconds) of arisen requests
        /// </summary>
        public int? TimeoutSeconds { get; set; }
        /// <summary>
        /// The content type of arisen requests
        /// </summary>
        public ContentType? ContentType { get; set; }

        private object _bodyData;
        /// <summary>
        /// The body data of arisen requests <br/>
        /// Originally, this property is null since we can not refer to your custom class for deserializing <br/>
        /// This property is only useful if <see cref="Method"/> is {<see cref="HttpMethod.Post"/>, <see cref="HttpMethod.Put"/>, <see cref="HttpMethod.Patch"/>}
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

        protected BaseRequest(string url, string absoluteUrl)
        {
            _url = url;
            AbsoluteUrl = absoluteUrl;
        }

        public static TRequest Create()
        {
            var request = new TRequest();
            return request;
        }

        protected void Init()
        {
            // Make the param container triggers to rebuild the Absolute URL whenever it's modified
            Params.OnChangedParams += RebuildUrl;
        }

        // Gen this constructor
        protected void RebuildUrl()
        {
            AbsoluteUrl = IUrlBuilder.Current.BuildUrl(_url, Params.ParamMapper);
        }

        protected virtual void SetRequestData<TResponse>(IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            if (RedirectsLimit.HasValue)
                requestAdaptor.RedirectLimit = RedirectsLimit.Value;
            if (TimeoutSeconds.HasValue)
                requestAdaptor.TimeoutSeconds = TimeoutSeconds.Value;
            if (ContentType is not null)
                requestAdaptor.ContentType = ContentType;
            requestAdaptor.Method = Method;
            foreach (var (k, v) in Headers)
                requestAdaptor.SetHeader(k, v);
        }
        private IEnumerator SetRequestDataAndWait<TResponse>(
            IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            SetRequestData(requestAdaptor);
            yield return requestAdaptor.RequestInstruction;
        }
    }
}