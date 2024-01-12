using System;
using System.Collections;
using System.Linq;
using System.Net.Mime;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Models;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.Request;
using SummerRest.Runtime.RequestAdaptor;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Editor.Manager
{
    public class EditorRequest : BaseRequest<EditorRequest>
    {
        private readonly Request _request;
        public override string SerializedBody { get; }

        protected override void SetRequestData<TResponse>(IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            base.SetRequestData(requestAdaptor);
            if (_request.AuthContainer is null) 
                return;
            AuthKey = _request.AuthContainer.AuthKey;
            if (string.IsNullOrEmpty(AuthKey))
                return;
            var appender = IGenericSingleton.GetSingleton<IAuthAppender, BearerTokenAuthAppender>(_request.AuthContainer.Appender);
            appender.Append(AuthKey, requestAdaptor);
        }
        
        protected EditorRequest(Request request) : base(request.Url,request.UrlWithParams)
        {
            _request = request;
            AbsoluteUrl = request.UrlWithParams;
            RedirectLimit = request.RedirectsLimit;
            TimeoutSeconds = request.TimeoutSeconds;
            Method = request.Method;
            ContentType = request.ContentType;
            if (request.Headers is not null)
                foreach (var header in request.Headers)
                    Headers.Add(header);
            SerializedBody = request.SerializedBody;
        }

        public static EditorRequest Create(Request request) => new EditorRequest(request);

        public EditorRequest() : base(string.Empty, string.Empty)
        {
        }

        public IEnumerator MakeRequest(Action callback)
        {
            var response = _request.LatestResponse;
            response.Clear();
            yield return DetailedRequestCoroutine<string>(r =>
            {
                if (r.WrappedRequest is not UnityWebRequest handler)
                    return;
                response.StatusCode = r.StatusCode;
                response.Headers = r.Headers.Select(e => (KeyValue)e).ToArray();

                var body = response.Body;
                var mediaType = response.Body.MediaType = r.ContentType.MediaType;
                body.MediaType = mediaType;
                body.FileName = DefaultContentTypeParser.ExtractFileName(r.Headers.FirstOrDefault(e => e.Key == "Content-Disposition").Value);
                body.RawBody = r.RawData;
                if (mediaType.StartsWith("image/") || mediaType.StartsWith("audio/") ||
                    mediaType == MediaTypeNames.Application.Octet)
                {
                    body.RawBytes.SetData(mediaType.StartsWith("image/"), handler.downloadHandler.data);
                }
            }, r =>
            {
                response.Error = r;
            });
            callback?.Invoke();
        }
    }
}