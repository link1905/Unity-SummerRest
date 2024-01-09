using System;
using System.Xml;
using Newtonsoft.Json;
using SummerRest.Utilities.RequestComponents;
using UnityEngine;
using Formatting = Newtonsoft.Json.Formatting;

namespace SummerRest.Runtime.Parsers
{
    public class DefaultDataSerializer : IDataSerializer
    {
        public static T StaticDeserialize<T>(string data, DataFormat dataFormat)
        {
            try
            {

                switch (dataFormat)
                {
                    case DataFormat.Json or DataFormat.PlainText:
                        if (data is T str)
                            return str;
                        return JsonConvert.DeserializeObject<T>(data);
                    case DataFormat.Bson:
                        break;
                    case DataFormat.Xml:
                        // Convert xml node to json
                        var doc = new XmlDocument();
                        doc.LoadXml(data);
                        var json = JsonConvert.SerializeXmlNode(doc.DocumentElement, Formatting.None, true);
                        // Deserialize json to data
                        return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (Exception)
            {
                // Debug.LogException(e);
            }
            Debug.LogWarningFormat("Can not deserialize {0} of type {1} by format {2} => return default value of {1}", data, typeof(T), dataFormat);
            return default;
        }

        public T Deserialize<T>(string data, DataFormat dataFormat)
            => StaticDeserialize<T>(data, dataFormat);
        public static string StaticSerialize<T>(T data, DataFormat dataFormat)
        {
            if (data is null)
                return default;
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
                        RootName);
                    return node.OuterXml;
            }
            return default;
        }

        private const string RootName = "root";
        public string Serialize<T>(T data, DataFormat dataFormat)
            => StaticSerialize(data, dataFormat);
    }
}