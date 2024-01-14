using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.Parsers
{
    public interface IDataSerializer : IDefaultSupport<IDataSerializer, DefaultDataSerializer>
    {
        T Deserialize<T>(string data, DataFormat dataFormat);
        string Serialize<T>(T data, DataFormat dataFormat);
    }
}