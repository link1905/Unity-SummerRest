using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.Parsers
{
    /// <summary>
    /// Used to serialize and deserialize requests and responses body data. Inherit this interface to use your custom serializer  <br/>
    /// Default support is <see cref="DefaultDataSerializer"/> based on Newtonsoft, you can change it to your own serializer in the plugin window
    /// </summary>
    public interface IDataSerializer : IDefaultSupport<IDataSerializer, DefaultDataSerializer>
    {
        T Deserialize<T>(string data, DataFormat dataFormat);
        string Serialize<T>(T data, DataFormat dataFormat);
    }
}