using SummerRest.Utilities.DataStructures;
using SummerRest.Utilities.RequestComponents;

namespace SummerRest.Runtime.Parsers
{
    public interface IDataSerializer : IDefaultSupport<IDataSerializer, DefaultDataSerializer>
    {
        T Deserialize<T>(string data, DataFormat dataFormat);
        string Serialize<T>(T data, DataFormat dataFormat);
    }
}