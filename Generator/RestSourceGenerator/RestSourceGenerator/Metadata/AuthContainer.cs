using System.Xml.Serialization;

namespace RestSourceGenerator.Metadata
{
    public struct AuthContainer
    {
        [XmlAttribute] public string AuthKey { get; set; }
        [XmlAttribute] public string AppenderType { get; set; }
        [XmlAttribute] public string AuthDataType { get; set; }
    }
}