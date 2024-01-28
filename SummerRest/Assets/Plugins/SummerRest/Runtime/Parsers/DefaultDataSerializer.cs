using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Runtime.Parsers
{
    public class DefaultDataSerializer : IDataSerializer
    {
        public static T StaticDeserialize<T>(string data, DataFormat dataFormat)
        {
            try
            {
                if (data is T str)
                    return str;
                switch (dataFormat)
                {
                    case DataFormat.Json or DataFormat.PlainText:
                        return JsonUtility.FromJson<T>(data);
                    case DataFormat.Xml:
                        var serializer = new XmlSerializer(typeof(T));
                        using (var reader = new StringReader(data))
                        {
                            return (T)serializer.Deserialize(reader);
                        }
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

        private readonly XmlWriterSettings _settings = new()
        {
            OmitXmlDeclaration = true,
        }; 
        private readonly XmlWriterSettings _beautySettings = new()
        {
            OmitXmlDeclaration = true, 
            Indent = true,
            IndentChars = "    ", // Use spaces for indentation, adjust as needed
            NewLineChars = "\n",   // Use newline character for line breaks, adjust as needed
            NewLineHandling = NewLineHandling.Replace,
        }; 
        public string Serialize<T>(T data, DataFormat dataFormat, bool beauty = false)
        {
            if (data is null)
                return default;
            if (data is string str)
                return str;
            switch (dataFormat)
            {
                case DataFormat.Json:
                    return JsonUtility.ToJson(data, beauty);
                case DataFormat.Xml:
                    XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
                    using (var sww = new StringWriter())
                    {
                        var settings = beauty ? _beautySettings : _settings;
                        using var xmlWriter = XmlWriter.Create(sww, settings);
                        xsSubmit.Serialize(xmlWriter, data);
                        return sww.ToString();
                    }
            }
            return default;
        }
    }
}