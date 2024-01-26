using System;
using System.Collections;
using System.Linq;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEngine.Networking;

namespace SummerRest.Editor.Requests
{
    /// <summary>
    /// Editor-only requester that reflects on <see cref="Request"/>
    /// </summary>
    public class EditorRequest : BaseRequest<EditorRequest>
    {
        private readonly Request _request;
        internal override string SerializedBody { get; }

        protected override void SetRequestData<TResponse>(IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            base.SetRequestData(requestAdaptor);
            if (_request.AuthContainer is null) 
                return;
            var authKey = _request.AuthContainer.AuthKey;
            if (string.IsNullOrEmpty(authKey))
                return;
            var appenderType = _request.AuthContainer.Appender;
            if (appenderType is null)
                return;
            var appender = Activator.CreateInstance(appenderType); 
            appender.CallGenericMethod(nameof(IAuthAppender<BearerTokenAuthAppender, string>.Append), new []
            {
                typeof(TResponse)
            }, _request.AuthContainer.GetData(), requestAdaptor);
        }
        
        protected EditorRequest(Request request) : base(request.Url,request.UrlWithParams)
        {
            _request = request;
            AbsoluteUrl = request.UrlWithParams;
            RedirectsLimit = request.RedirectsLimit;
            TimeoutSeconds = request.TimeoutSeconds;
            Method = request.Method;
            ContentType = request.ContentType;
            if (request.Headers is not null)
                foreach (var header in request.Headers)
                    Headers.Add(header);
            SerializedBody = request.SerializedBody;
        }

        public static EditorRequest Create(Request request) => new(request);

        public EditorRequest() : base(string.Empty, string.Empty)
        {
        }

        private void SetResponseCallback(WebResponse<string> r)
        {
            var response = _request.LatestResponse;
            if (r.WrappedRequest is not UnityWebRequest handler)
                return;
            // Set response data
            response.StatusCode = r.StatusCode;
            response.Headers = r.Headers.Select(e => (KeyValue)e).ToArray();
            var body = response.Body;
            var mediaType = response.Body.MediaType = r.ContentType.MediaType;
            body.MediaType = mediaType;
            body.FileName = DefaultContentTypeParser.ExtractFileName(r.Headers.FirstOrDefault(e => e.Key == "Content-Disposition").Value);
            body.RawBody = r.RawData;
            if (mediaType.StartsWith("image/") || mediaType.StartsWith("audio/") ||
                mediaType == Runtime.RequestComponents.ContentType.MediaTypeNames.Application.Octet)
            {
                body.RawBytes.SetData(mediaType.StartsWith("image/"), handler.downloadHandler.data);
            }
        }

        public IEnumerator MakeRequest(Action callback)
        {
            var response = _request.LatestResponse;
            response.Clear();
            var contentType = _request.ContentType ?? new ContentType();
            IEnumerator request;
            switch (contentType.MediaType)
            {
                case var multipartType when multipartType.StartsWith("multipart/"):
                {
                    request = DetailedRequestCoroutine<string>(SetResponseCallback, r => response.Error = r);
                    break;
                }
                default:
                {
                    request = DetailedMultipartFileRequestCoroutine<string>(SetResponseCallback, r => response.Error = r);
                    break;
                }
            }
            yield return request;
            callback?.Invoke();
        }
    }
}