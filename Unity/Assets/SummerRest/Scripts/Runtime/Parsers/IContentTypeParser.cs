using SummerRest.Scripts.Utilities.RequestComponents;

namespace SummerRest.Runtime.Parsers
{
    public interface IContentTypeParser : IDefaultSupport<IContentTypeParser, DefaultContentTypeParser>
    {
        string ContentTypeHeaderKey { get; }
        DataFormat ParseDataFormatFromResponse(string contentTypeHeader);
        ContentType ParseContentTypeFromHeader(string contentTypeHeader);
    }
}