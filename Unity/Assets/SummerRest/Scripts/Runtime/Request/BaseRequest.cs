using System;
using System.Collections;
using System.Collections.Generic;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.Request
{
    public class RequestParamContainer
    {
        private readonly Dictionary<string, ICollection<string>> _paramMapper = new();
        public IDictionary<string, ICollection<string>> ParamMapper => _paramMapper;
        public event Action OnChangedParams;
        public void AddParam(string key, string value)
        {
            if (!_paramMapper.TryGetValue(key, out var values))
            {
                values = new HashSet<string>();
                _paramMapper.Add(key, values);
            }
            values.Add(value);
            OnChangedParams?.Invoke();
        }
        public bool RemoveParam(string key)
        {
            if (!_paramMapper.Remove(key)) 
                return false;
            OnChangedParams?.Invoke();
            return true;
        }
        public bool RemoveValueFromParam(string key, string value)
        {
            if (!_paramMapper.TryGetValue(key, out var values))
                return false;
            var rev = values.Remove(value);
            if (values.Count == 0)
                _paramMapper.Remove(key);
            if (rev)
                OnChangedParams?.Invoke();
            return rev;
        }
        public void AddParams(string key, IEnumerable<string> addValues)
        {
            if (!_paramMapper.TryGetValue(key, out var values))
            {
                values = new HashSet<string>(addValues);
                _paramMapper.Add(key, values);
            }
            else
            {
                foreach (var value in addValues)
                    values.Add(value);
            }
            OnChangedParams?.Invoke();
        }
    }
    public abstract class BaseRequest<TRequest> : IWebRequest where TRequest : BaseRequest<TRequest>, new()
    {
        private string _absoluteUrl;
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
        public int? RedirectLimit { get; set; }
        public int? TimeoutSeconds { get; set; }
        // public IAuthData AuthData { get; set; }
        public ContentType ContentType { get; set; }
        protected string SerializedBody;
        protected BaseRequest(string url)
        {
            _url = url;
        }
        public static TRequest Create()
        {
            var request = new TRequest();
            return request;
        }
        protected void Init()
        {
            Params.OnChangedParams += RebuildUrl;
            RebuildUrl();
        }
        // Gen this constructor
        protected void RebuildUrl()
        {
            _absoluteUrl = IUrlBuilder.Current.BuildUrl(_url, Params.ParamMapper);
        }
        private IEnumerator SetRequestDataAndWait<TResponse>(
            IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            if (RedirectLimit.HasValue)
                requestAdaptor.RedirectLimit = RedirectLimit.Value;
            if (TimeoutSeconds.HasValue)
                requestAdaptor.TimeoutSeconds = TimeoutSeconds.Value;
            if (ContentType is not null)
                requestAdaptor.ContentType = ContentType;
            foreach (var (k, v) in Headers)
                requestAdaptor.SetHeader(k, v);
            yield return requestAdaptor.RequestInstruction;
        }
    
        private IEnumerator SimpleResponseCoroutine<TResponse>(IWebRequestAdaptor<TResponse> request, 
            Action<TResponse> doneCallback)
        {
            yield return SetRequestDataAndWait(request);
            doneCallback?.Invoke(request.ResponseData);
        }
        public IEnumerator SimpleResponseCoroutine<TResponse>(UnityWebRequest webRequest, 
            Action<TResponse> doneCallback)
        {
            using var request = WebRequestProvider.GetFromUnityWebRequest<TResponse>(webRequest);
            yield return SimpleResponseCoroutine(request, doneCallback);
        }
        public IEnumerator SimpleResponseCoroutine(Action<Texture2D> doneCallback, bool readable)
        {
            using var request = WebRequestProvider.GetTextureRequest(_absoluteUrl, readable);
            yield return SimpleResponseCoroutine(request, doneCallback);
        }
        public IEnumerator SimpleResponseCoroutine(Action<AudioClip> doneCallback, AudioType audioType)
        {
            using var request = WebRequestProvider.GetAudioRequest(_absoluteUrl, audioType);
            yield return SimpleResponseCoroutine(request, doneCallback);
        }
        public IEnumerator SimpleResponseCoroutine<TBody>(Action<TBody> doneCallback)
        {
            using var request = WebRequestProvider.GetDataRequest<TBody>(_absoluteUrl, Method, SerializedBody);
            yield return SimpleResponseCoroutine(request, doneCallback);
        }
    
        private IEnumerator DetailedResponseCoroutine<TResponse>(IWebRequestAdaptor<TResponse> request, 
            Action<IWebResponse<TResponse>> doneCallback)
        {
            yield return SetRequestDataAndWait(request);
            doneCallback?.Invoke(request.WebResponse);
        }
        public IEnumerator DetailedResponseCoroutine<TResponse>(UnityWebRequest webRequest, 
            Action<IWebResponse<TResponse>> doneCallback)
        {
            using var request = UnityWebRequestAdaptor<TResponse>.Create(webRequest);
            yield return DetailedResponseCoroutine(request, doneCallback);
        }
        public IEnumerator DetailedResponseCoroutine(Action<IWebResponse<Texture2D>> doneCallback, bool readable)
        {
            using var request = WebRequestProvider.GetTextureRequest(_absoluteUrl, readable);
            yield return DetailedResponseCoroutine(request, doneCallback);
        }
        public IEnumerator DetailedResponseCoroutine(Action<IWebResponse<AudioClip>> doneCallback, AudioType audioType)
        {
            using var request = WebRequestProvider.GetAudioRequest(_absoluteUrl, audioType);
            yield return DetailedResponseCoroutine(request, doneCallback);
        }
        public IEnumerator DetailedResponseCoroutine<TBody>(Action<IWebResponse<TBody>> doneCallback)
        {
            using var request = WebRequestProvider.GetDataRequest<TBody>(_absoluteUrl, Method, SerializedBody);
            yield return DetailedResponseCoroutine(request, doneCallback);
        }
    }

    public abstract class BaseRequest<TRequest, TRequestBody> : BaseRequest<TRequest>, IWebRequest<TRequestBody>
        where TRequest : BaseRequest<TRequest>, new()
    {
        private TRequestBody _requestBody;

        public TRequestBody BodyData
        {
            get => _requestBody;
            set
            {
                _requestBody = value;
                RebuildBody();
            }
        }

        private DataFormat _bodyFormat;

        public DataFormat BodyFormat
        {
            get => _bodyFormat;
            set
            {
                _bodyFormat = value;
                RebuildBody();
            }
        }

        private void RebuildBody()
        {
            if (BodyData is null)
                return;
            SerializedBody = IDataSerializer.Current.Serialize(BodyData, BodyFormat);
        }

        protected BaseRequest(string url) : base(url)
        {
        }
    }
}