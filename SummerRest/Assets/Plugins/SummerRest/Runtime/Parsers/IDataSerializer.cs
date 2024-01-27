using SummerRest.Runtime.Attributes;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.Parsers
{
    /// <summary>
    /// Used to serialize and deserialize requests and responses body data. Inherit this interface to use your custom serializer  <br/>
    /// Default support is <see cref="DefaultDataSerializer"/> based on Newtonsoft, you can change it to your own serializer in the plugin window
    /// </summary>
    [GeneratedDefault("DataSerializer", typeof(DefaultDataSerializer))]
    public partial interface IDataSerializer
    {
        /// <summary>
        /// Deserialize a text to a custom data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataFormat"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Deserialize<T>(string data, DataFormat dataFormat);
        /// <summary>
        /// Serialize a custom data to a text
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataFormat"></param>
        /// <param name="beauty">Should the text look pretty. You can ignore this parameter because it is intentionally used to show the apparent text in the editor</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string Serialize<T>(T data, DataFormat dataFormat, bool beauty = false);
    }
}