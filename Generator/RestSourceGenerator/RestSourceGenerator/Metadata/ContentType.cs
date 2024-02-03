using System.Xml.Serialization;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Metadata
{
    public struct ContentType
    {
        [XmlAttribute]
        public string Charset { get; set; }
        [XmlAttribute]
        public string MediaType { get; set; }
        [XmlAttribute]
        public string Boundary { get; set; }

        public ContentType(string charset, string mediaType, string boundary)
        {
            Charset = charset;
            MediaType = mediaType;
            Boundary = boundary;
        }

        public string ToInstance()
        {
            return
                $@"new {nameof(ContentType)}({MediaType.GetRefToPredefinedValue()}, {Charset.GetRefToPredefinedValue()}, {Boundary.GetRefToPredefinedValue()})";
        }

        public class MediaTypeNames
        {
            
        }

    }
}