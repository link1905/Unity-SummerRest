using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace Tests.Behaviour.Scripts
{
    public class LogDataSerializer : IDataSerializer
    {
        private readonly IDataSerializer _wrapped = new DefaultDataSerializer();
        public T Deserialize<T>(string data, DataFormat dataFormat)
        {
            Debug.LogFormat("Deserialize data with format {0}", dataFormat);
            return _wrapped.Deserialize<T>(data, dataFormat);
        }

        public string Serialize<T>(T data, DataFormat dataFormat, bool beauty = false)
        {
            Debug.LogFormat("Serialize data with format {0}", dataFormat);
            return _wrapped.Serialize(data, dataFormat, beauty);
        }
    }
}