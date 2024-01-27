using System;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using SummerRest.Runtime.RequestComponents;
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

        public string Serialize<T>(T data, DataFormat dataFormat, bool beauty = false)
        {
            if (data is null)
                return default;
            if (data is string str)
                return str;
            var format = beauty ? Formatting.Indented : Formatting.None;
            switch (dataFormat)
            {
                case DataFormat.Json:
                    return JsonConvert.SerializeObject(data, format);
                case DataFormat.Xml:
                    var node = JsonConvert.DeserializeXmlNode(
                        JsonConvert.SerializeObject(data), 
                        RootName);
                    if (beauty)
                        return GetIndentedXmlString(node);
                    return node.OuterXml;
            }
            return default;
        }

        private static string GetIndentedXmlString(XmlDocument xmlDoc)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ", // Use spaces for indentation, adjust as needed
                NewLineChars = "\n",   // Use newline character for line breaks, adjust as needed
                NewLineHandling = NewLineHandling.Replace,
                OmitXmlDeclaration = true // Set to true if you don't want an XML declaration
            };
            using var stringWriter = new StringWriter();
            using (var xmlWriter = XmlWriter.Create(stringWriter, settings)) 
                xmlDoc.Save(xmlWriter);
            return stringWriter.ToString();
        }
        private const string RootName = "root";
    }
}