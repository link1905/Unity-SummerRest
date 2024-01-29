using System.Xml.Serialization;

namespace RestSourceGenerator.Metadata
{
    [XmlRoot("SummerRestConfiguration")]
    public struct Configuration
    {
        [XmlAttribute]
        public string Assembly { get; set; }
        [XmlArray, XmlArrayItem("Domain")]
        public Request[]? Domains { get; set; }
    }
}