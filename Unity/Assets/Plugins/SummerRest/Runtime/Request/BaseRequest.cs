using System.Collections;
using System.Collections.Generic;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.Request
{
    public abstract partial class BaseRequest<TRequest> : IWebRequest where TRequest : BaseRequest<TRequest>, new() 
    {
        public string AbsoluteUrl { get; protected set; }
        private string _url;

        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                RebuildUrl();
            }
        }

        public RequestParamContainer Params { get; } = new();

        public IDictionary<string, string> Headers { get; } =
            new Dictionary<string, string>();

        public HttpMethod Method { get; set; }
        public int? RedirectsLimit { get; set; }
        public int? TimeoutSeconds { get; set; }
        public string AuthKey { get; set; }
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