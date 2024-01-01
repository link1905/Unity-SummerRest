using SummerRest.Scripts.Utilities.RequestComponents;

namespace SummerRest.Runtime.Parsers
{
    public interface IDataSerializer : IDefaultSupport<IDataSerializer, NewtonSoftDataSerializer>
    {
        T Deserialize<T>(string data, DataFormat dataFormat);
        string Serialize<T>(T data, DataFormat dataFormat);
    }
}