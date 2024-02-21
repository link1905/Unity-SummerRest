using NUnit.Framework;
using SummerRest.Runtime.Authenticate.Repositories;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor
{
    // These tests depends on the plugin
    // Since we have changed the secretRepository and dataSerializer default classes, it should use correct instances
    public class CustomGeneratedBehaviourTest
    {
        [Test]
        public void Test_Log_Secret_Repository_Should_Log()
        {
            ISecretRepository.Current.Save("my-key", "my-value");
            LogAssert.Expect(LogType.Log, @"Save ""my-key"" key");
            ISecretRepository.Current.Delete("my-key");
            LogAssert.Expect(LogType.Log, @"Delete ""my-key"" key");
            ISecretRepository.Current.TryGet<string>("my-key", out _);
            LogAssert.Expect(LogType.Log, @"Key ""my-key"" does not exist");
        }
        [Test]
        public void Test_Log_Data_Serializer_Should_Log()
        {
            IDataSerializer.Current.Serialize("my-data", DataFormat.Json);
            LogAssert.Expect(LogType.Log, "Serialize data with format Json");
            IDataSerializer.Current.Deserialize<string>("my-data", DataFormat.Json);
            LogAssert.Expect(LogType.Log, "Deserialize data with format Json");
        }
    }
}