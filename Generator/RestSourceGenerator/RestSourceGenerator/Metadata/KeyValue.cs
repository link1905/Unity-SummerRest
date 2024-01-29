using System.Xml.Serialization;

namespace RestSourceGenerator.Metadata
{
    public struct KeyValue
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
    }
}