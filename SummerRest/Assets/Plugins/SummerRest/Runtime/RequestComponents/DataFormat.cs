using System;

namespace SummerRest.Runtime.RequestComponents
{
    /// <summary>
    /// Data format for serializing requests and deserializing responses (only for raw text responses)
    /// </summary>
    [Serializable]
    public enum DataFormat
    {
        /// <summary>
        /// Treat the data as JSON string https://www.json.org/json-en.html
        /// </summary>
        Json = 0, 
        /// <summary>
        /// Do nothing with the body
        /// </summary>
        PlainText = 1, 
        /// <summary>
        /// XML format commonly used in SOAP servers
        /// </summary>
        Xml = 3, 
    }
}