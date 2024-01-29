using System.Xml.Serialization;

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
    }
}