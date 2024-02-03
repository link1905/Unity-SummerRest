using System.Xml.Serialization;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Metadata
{
    public struct KeyValue
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public string Value { get; set; }

        public KeyValue(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public (string key, string value) Deconstruct()
        {
            return (Key, Value);
        }
        public (string key, string value) DeconstructEmbedded()
        {
            return (Key, Value.ToEmbeddedString());
        }
        public KeyValue ToKeyWithEmbeddedValue()
        {
            return new KeyValue
            {
                Key = Key,
                Value = Value.ToEmbeddedString()
            };
        }
    }
}