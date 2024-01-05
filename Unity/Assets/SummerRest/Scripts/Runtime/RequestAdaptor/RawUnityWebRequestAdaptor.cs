using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.Request;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    public class RawUnityWebRequestAdaptor<TResponse> : UnityWebRequestAdaptor<RawUnityWebRequestAdaptor<TResponse>, TResponse>
    {
        private string _rawResponse;
        protected override void DoneRequest()
        {
            _rawResponse = WebRequest.downloadHandler.text;
            ResponseData = BuildResponse(WebRequest, _rawResponse);
        }
        public TResponse BuildResponse(string header, string response)
        {
            var parser = IContentTypeParser.Current;
            var dataFormatFromResponse =
                parser.ParseDataFormatFromResponse(header);
            return IDataSerializer.Current.Deserialize<TResponse>(
                response,
                dataFormatFromResponse);
        }

        private TResponse BuildResponse(UnityWebRequest request, string rawResponse)
        {
            if (request.result != UnityWebRequest.Result.Success)
                return default;
            var parser = IContentTypeParser.Current;
            return BuildResponse(request.GetResponseHeader(parser.ContentTypeHeaderKey), rawResponse);
        }
        public override IWebResponse<TResponse> WebResponse => new UnityWebResponse(WebRequest, _rawResponse, ResponseData);
        public override void Dispose()
        {
            _rawResponse = default;
            base.Dispose();
        }
    }
}