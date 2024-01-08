using System;
using NUnit.Framework;
using SummerRest.Runtime.Parsers;
using SummerRest.Scripts.Utilities.Extensions;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine;
using UnityEngine.TestTools;

namespace SummerRest.Scripts.Tests
{

    public class SerializerTest
    {
        public class TestModel
        {
            public string A { get; set; }
            public int B { get; set; }
            public float C { get; set; }
            public bool Equals(TestModel other)
            {
                return A == other.A && B == other.B && C.Equals(other.C);
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(A, B, C);
            }
        }
        [Test]
        public void Test_Serialize_To_Json()
        {
            var json = @"
{
  ""A"": ""5"",
  ""B"": 5,
  ""C"": 5.5
}
".RemoveEscapeChar();
            var model = new TestModel
            {
                A = "5",
                B = 5,
                C = 5.5f
            };
            var result = IDataSerializer.Current.Serialize(model, DataFormat.Json);
            Assert.AreEqual(json, result);
        }
        [Test]
        public void Test_Serialize_To_Xml()
        {
            var xml = @"
<root>
	<A>5</A>
	<B>5</B>
	<C>5.5</C>
</root>
".RemoveEscapeChar(); 
            var model = new TestModel
            {
                A = "5",
                B = 5,
                C = 5.5f
            };
            var result = IDataSerializer.Current.Serialize(model, DataFormat.Xml);
            Assert.AreEqual(xml, result);
        }
        [Test]
        public void Test_Serialize_To_Plain_Return_Null()
        {
            var model = new TestModel();
            var result = IDataSerializer.Current.Serialize(model, DataFormat.PlainText);
            Assert.IsNull(result);
        }
        [Test]
        public void Test_Serialize_From_Null_Return_Null()
        {
            var result = IDataSerializer.Current.Serialize<string>(null, DataFormat.Json);
            Assert.IsNull(result);
        }
        [Test]
        public void Test_Serialize_String_Then_Return_The_String_Itself()
        {
            const string model = "no need to be serialized";
            var result = IDataSerializer.Current.Serialize(model, DataFormat.Json);
            Assert.AreSame(model, result);
            result = IDataSerializer.Current.Serialize(model, DataFormat.Xml);
            Assert.AreSame(model, result);
            result = IDataSerializer.Current.Serialize(model, DataFormat.PlainText);
            Assert.AreSame(model, result);
        }
        [Test]
        public void Test_Deserialize_To_Json()
        {
            const string json = @"
{
  ""A"": ""5"",
  ""B"": 5,
  ""C"": 5.5
}
";
            var model = new TestModel
            {
                A = "5",
                B = 5,
                C = 5.5f
            };
            var result = IDataSerializer.Current.Deserialize<TestModel>(json, DataFormat.Json);
            Assert.That(model.Equals(result));
        }
        [Test]
        public void Test_Deserialize_To_Json_Fail_Return_Null_Log_Error()
        {
            const string json = @"
{
  ""A"": ""5""
  ""B"": 5
}
";
            var result = IDataSerializer.Current.Deserialize<TestModel>(json, DataFormat.Json);
            Assert.IsNull(result);
            LogAssert.Expect(LogType.Warning,
                $"Can not deserialize {json} of type {typeof(TestModel)} by format {DataFormat.Json} => return default value of {typeof(TestModel)}");
        }
        [Test]
        public void Test_Deserialize_To_Xml()
        {
            const string xml = @"
<root>
	<A>5</A>
	<B>5</B>
	<C>5.5</C>
</root>
";
            var model = new TestModel
            {
                A = "5",
                B = 5,
                C = 5.5f
            };
            var result = IDataSerializer.Current.Deserialize<TestModel>(xml, DataFormat.Xml);
            Assert.That(model.Equals(result));
        }
        [Test]
        public void Test_Deserialize_To_Plain_Model_Then_Returns_Null()
        {
            const string json = "my json";
            var result = IDataSerializer.Current.Deserialize<TestModel>(json, DataFormat.PlainText);
            Assert.IsNull(result);
        }
        [Test]
        public void Test_Deserialize_To_String_From_String_Then_Return_The_String_Itself()
        {
            const string json = "my json";
            var result = IDataSerializer.Current.Deserialize<string>(json, DataFormat.PlainText);
            Assert.AreSame(result, json);
        }
    }
}