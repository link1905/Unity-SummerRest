using System.Runtime.CompilerServices;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using UnityEngine.Networking;

[assembly: InternalsVisibleTo("SummerRest.Tests")]
namespace SummerRest.Runtime.RequestAdaptor
{
    /// <summary>
    /// Request adaptor deserializing the response body to C# class objects based on the content-type <seealso cref="DataFormat"/> 
    /// </summary>
    internal class RawUnityWebRequestAdaptor<TResponse> : UnityWebRequestAdaptor<RawUnityWebRequestAdaptor<TResponse>, TResponse>
    {
        private string _rawResponse;
        public override string RawResponse => _rawResponse;
        internal override TResponse BuildResponse()
        {
            _rawResponse = WebRequest.downloadHandler.text;
            return BuildResponse(WebRequest, _rawResponse);
        }
        internal TResponse BuildResponse(string header, string response)
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
        public override void Dispose()
        {
            _rawResponse = default;
            base.Dispose();
        }
    }
}