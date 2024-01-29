using System;
using NUnit.Framework;
using SummerRest.Runtime.Extensions;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;
using UnityEngine.TestTools;

namespace SummerRest.Tests
{

    public class SerializerTest
    {
        [Serializable]
        public class TestModel
        {
            public string a;
            public int b;
            public float c;

            public TestModel()
            {
                
            }
            public TestModel(string a, int b, float c)
            {
                this.a = a;
                this.b = b;
                this.c = c;
            }
            public bool Equals(TestModel other)
            {
                return a == other.a && b == other.b && c.Equals(other.c);
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(a, b, c);
            }
        }
        [Test]
        public void Test_Serialize_To_Json()
        {
            var json = @"
{
  ""a"": ""5"",
  ""b"": 5,
  ""c"": 5.5
}
".RemoveEscapeChar();
            var model = new TestModel("5", 5, 5.5f);
            var result = IDataSerializer.Current.Serialize(model, DataFormat.Json);
            Assert.AreEqual(json, result);
        }
        [Test]
        public void Test_Serialize_To_Xml()
        {
            var xml = @"
<TestModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""> 
	<a>5</a>
	<b>5</b>
	<c>5.5</c>
</TestModel>
".RemoveEscapeChar(); 
            var model = new TestModel("5", 5, 5.5f);
            var result = IDataSerializer.Current.Serialize(model, DataFormat.Xml);
            Assert.AreEqual(xml, result.RemoveEscapeChar());
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
  ""a"": ""5"",
  ""b"": 5,
  ""c"": 5.5
}
";
            var model = new TestModel("5", 5, 5.5f);
            var result = IDataSerializer.Current.Deserialize<TestModel>(json, DataFormat.Json);
            Assert.That(model.Equals(result));
        }
        [Test]
        public void Test_Deserialize_To_Json_Fail_Return_Null_Log_Error()
        {
            const string json = @"
{
  ""a"": ""5""
  ""b"": 5
}
";
            var result = IDataSerializer.Current.Deserialize<TestModel>(json, DataFormat.Json);
            Assert.IsNull(result);
            LogAssert.Expect(LogType.Error,
                $"JSON parse error: Missing a comma or '}}' after an object member.\nCould not deserialize {json} of type {typeof(TestModel)} by format {DataFormat.Json} => return default value of {typeof(TestModel)}");
        }
        [Test]
        public void Test_Deserialize_To_Xml()
        {
            const string xml = @"
<TestModel>
	<a>5</a>
	<b>5</b>
	<c>5.5</c>
</TestModel>
";
            var model = new TestModel("5", 5, 5.5f);

            var result = IDataSerializer.Current.Deserialize<TestModel>(xml, DataFormat.Xml);
            Assert.That(model.Equals(result));
        }
  
        [Test]
        public void Test_Deserialize_To_Plain_Model_Then_Returns_Null()
        {
            const string json = "my json";
            var result = IDataSerializer.Current.Deserialize<TestModel>(json, DataFormat.PlainText);
            Assert.IsNull(result);
            LogAssert.Expect(LogType.Error,
                $"JSON parse error: Invalid value.\nCould not deserialize {json} of type {typeof(TestModel)} by format {DataFormat.PlainText} => return default value of {typeof(TestModel)}");
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