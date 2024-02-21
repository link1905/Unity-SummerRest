using NUnit.Framework;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;

namespace Tests.Editor
{
    public class ContentTypeParserTest
    {
        private static readonly IContentTypeParser ContentTypeParser = new DefaultContentTypeParser();        
        [Test]
        public void Test_Content_Type_To_Formed_Text()
        {
            var table = new (ContentType contentType, string text)[]
            {
                (new ContentType(charset: string.Empty, mediaType: "application/json"), "application/json"),
                (new ContentType(charset: "utf-8",  mediaType: "application/xml"), "application/xml; charset=utf-8"),
                (new ContentType(charset: "utf-8",  mediaType: "application/xml", boundary: "123456abcd"), "application/xml; charset=utf-8; boundary=123456abcd"),
                (new ContentType(charset: string.Empty,  mediaType: "text/plain", boundary: "www123"), "text/plain; boundary=www123"),
                (new ContentType(charset: string.Empty,  mediaType: "application/octet-stream", boundary: "www123"), "application/octet-stream; boundary=www123"),
            };
            foreach (var entry in table)
            {
                var result = entry.contentType.FormedContentType;
                Assert.AreEqual(result, entry.text);
            }
        }
        
        [Test]
        public void Test_Content_Type_From_Formed_Text()
        {
            var defaultParser = ContentTypeParser;
            var table = new (ContentType contentType, string text)[]
            {
                (new ContentType(charset: string.Empty, mediaType: "application/json"), "application/json"),
                (new ContentType(charset: "utf-8",  mediaType: "application/xml"), "application/xml; charset=utf-8"),
                (new ContentType(charset: "utf-8",  mediaType: "application/xml", boundary: "123456abcd"), "application/xml; charset=utf-8; boundary=123456abcd"),
                (new ContentType(charset: string.Empty,  mediaType: "text/plain", boundary: "www123"), "text/plain; boundary=www123"),
                (new ContentType(charset: string.Empty,  mediaType: "application/octet-stream", boundary: "www123"), "application/octet-stream; boundary=www123"),
                (new ContentType(charset: string.Empty,  mediaType: "text/plain", boundary: "www123"), "; boundary=www123"),
                (new ContentType(), string.Empty),
            };
            foreach (var entry in table)
            {
                var result = defaultParser.ParseContentTypeFromHeader(entry.text);
                Assert.That(result.Equals(entry.contentType), "{0} not equals to {1}", result.FormedContentType, entry.contentType.FormedContentType);
            }
        }
        
        
        [Test]
        public void Test_Pass_Get_Media_Type_From_ContentType()
        {
            var defaultParser = ContentTypeParser;
            var table = new (string contentType, DataFormat value)[]
            {
                ("application/json", DataFormat.Json),
                ("application/xml; charset=utf-8", DataFormat.Xml),
                ("text/plain; charset=utf-8; boundary=asdasdas", DataFormat.PlainText),
            };
            foreach (var entry in table)
            {
                var result = defaultParser.ParseDataFormatFromResponse(entry.contentType);
                Assert.AreEqual(result, entry.value);
            }
        }
        
        [Test]
        public void Test_Fail_Get_Media_Type_From_ContentType_Then_Use_PlainText()
        {
            var defaultParser = ContentTypeParser;
            var table = new string[]
            {
                ";charset=utf-8", 
                "multipart/form-data; charset=utf-8; boundary=asdasdas", 
                "charset=utf-8; boundary=asdasdas",
                "image/png"
            };
            foreach (var entry in table)
            {
                var result = defaultParser.ParseDataFormatFromResponse(entry);
                Assert.AreEqual(result, DataFormat.PlainText);
            }
        }
    }
}