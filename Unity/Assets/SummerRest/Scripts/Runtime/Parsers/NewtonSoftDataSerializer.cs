using System.Xml;
using Newtonsoft.Json;
using SummerRest.Scripts.Utilities.RequestComponents;

namespace SummerRest.Runtime.Parsers
{
    public class NewtonSoftDataSerializer : IDataSerializer
    {
        public T Deserialize<T>(string data, DataFormat dataFormat)
        {
            switch (dataFormat)
            {
                case DataFormat.PlainText:
                    return data is T str ? str : default;
                case DataFormat.Json:
                    return JsonConvert.DeserializeObject<T>(data);
                case DataFormat.Bson:
                    break;
                case DataFormat.Xml:
                    // Convert xml node to json
                    var doc = new XmlDocument();
                    doc.LoadXml(data);
                    var json = JsonConvert.SerializeXmlNode(doc);
                    // Deserialize json to data
                    return JsonConvert.DeserializeObject<T>(json);
            }
            return default;
        }

        public string Serialize<T>(T data, DataFormat dataFormat)
        {
            if (data is string str)
                return str;
            switch (dataFormat)
            {
                case DataFormat.Json:
                    return JsonConvert.SerializeObject(data);
                case DataFormat.Bson:
                    break;
                case DataFormat.Xml:
                    var node = JsonConvert.DeserializeXmlNode(
                        JsonConvert.SerializeObject(data), 
                        nameof(T));
                    return node.OuterXml;
            }
            return default;
        }
    }
}