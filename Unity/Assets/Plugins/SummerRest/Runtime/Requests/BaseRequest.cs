using System.Collections;
using System.Collections.Generic;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;

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
        /// Original value is your input in the plugin window <br/>
        /// Please note that, this property only works with data request; texture, audio... requests automatically use <see cref="HttpMethod.Get"/>
        /// </summary>
        public HttpMethod Method { get; set; }
        /// <summary>
        /// Original value is your input in the plugin window <br/>
        /// </summary>
        public int? RedirectsLimit { get; set; }
        /// <summary>
        /// Original value is your input in the plugin window <br/>
        /// </summary>
        public int? TimeoutSeconds { get; set; }
        /// <summary>
        /// Original value is your input in the plugin window <br/>
        /// </summary>
        public ContentType? ContentType { get; set; }
        public virtual string SerializedBody => null;

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