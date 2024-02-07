using System.Collections;
using System.Collections.Generic;
using SummerRest.Runtime.Authenticate.Appenders;
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
    public abstract partial class BaseRequest<TRequest> 
        where TRequest : BaseRequest<TRequest>, new()
    {
        /// <summary>
        /// Absolute URL built from <see cref="Url"/> and <see cref="Params"/> 
        /// </summary>
        public string AbsoluteUrl { get; private set; }
        /// <summary>
        /// Url of this request excluding request params <br/>
        /// Original value is your input in the plugin window
        /// </summary>
        public string Url { get; private set; }
        public string UrlFormat { get; }
        private readonly string[] _urlFormatValues;
        /// <summary>
        /// Get current embedded value at position <see cref="index"/>
        /// </summary>
        /// <param name="index">The formatted position. You should leverage the smart keys which you defined by accessing <see cref="Keys.UrlFormat"/></param>
        /// <returns></returns>
        public string GetUrlValue(int index) => _urlFormatValues[index];
        /// <summary>
        /// Set the value of the format url at position <see cref="index"/>
        /// </summary>
        /// <param name="index">The formatted position. You should leverage the smart keys which you defined by accessing <see cref="Keys.UrlFormat"/></param>
        /// <param name="value">New value</param>
        /// <returns></returns>
        public void SetUrlValue(int index, string value)
        {
            _urlFormatValues[index] = value;
            RebuildUrl(true);
        }

        /// <summary>
        /// Access this to modify your params in runtime<br/>
        /// You should leverage the keys which you defined by accessing <see cref="Keys.Params"/> <br/>
        /// Please note that, this property originally contains your inputs in the plugin window
        /// </summary>
        public RequestParamContainer Params { get; } = new();

        private readonly IRequestModifier _requestModifier;

        /// <summary>
        /// Access this to modify your headers in runtime<br/>
        /// You should leverage the keys which you defined by accessing <see cref="Keys.Headers"/> <br/>
        /// Please note that, this property originally contains your inputs in the plugin window
        /// </summary>
        public IDictionary<string, string> Headers { get; } =
            new Dictionary<string, string>();

        /// <summary>
        /// The method of arisen requests <br/>
        /// Please note that, this property only works with data request <br/>
        /// Texture, audio... requests automatically use <see cref="HttpMethod.Get"/>
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
        /// <summary>
        /// Used to resolve auth values in <see cref="Authenticate.Repositories.ISecretRepository"/> <seealso cref="IAuthData"/> <seealso cref="IAuthAppender{TAuthAppender,TAuthData}"/> <br/>
        /// You should leverage the key which you defined by accessing <see cref="Keys.AuthKey"/> <br/>
        /// </summary>
        public string AuthKey { get; set; }


        protected BaseRequest(string url, string absoluteUrl,  string urlFormat, string[] urlFormatValues, IRequestModifier requestModifier)
        {
            Url = url;
            AbsoluteUrl = absoluteUrl;
            UrlFormat = urlFormat;
            _urlFormatValues = urlFormatValues;
            _requestModifier = requestModifier;
        }

        public static TRequest Create()
        {
            var request = new TRequest();
            return request;
        }

        protected void Init()
        {
            // Make the param container triggers to rebuild the Absolute URL whenever it's modified
            Params.OnChangedParams += () => RebuildUrl(false);
        }

        // Gen this constructor
        protected void RebuildUrl(bool format)
        {
            if (format && !string.IsNullOrEmpty(UrlFormat) && _urlFormatValues.Length > 0)
                Url = string.Format(UrlFormat, _urlFormatValues);
            AbsoluteUrl = IUrlBuilder.Current.BuildUrl(Url, Params.ParamMapper);
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
            _requestModifier?.ModifyRequestData(this, requestAdaptor);
        }
        private IEnumerator SetRequestDataAndWait<TResponse>(
            IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            SetRequestData(requestAdaptor);
            yield return requestAdaptor.RequestInstruction;
        }
    }
}