using System.Collections.Generic;
using NUnit.Framework;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.Request;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Tests
{
    public class RequestBuilderTest
    {
        private const string ExampleUrl = "summerrest.com";
        private const string ExampleUrl1 = "summerrest1.com";
        [Test]
        public void Test_Build_Param()
        {
            var table = new (string url, IEnumerable<KeyValuePair<string, ICollection<string>>> parameters, string result)[]
            {
                (
                    ExampleUrl,
                    new []
                    {
                        new KeyValuePair<string, ICollection<string>>("id", new[] {"1"})
                    },
                    $"{ExampleUrl}?id=1"),
                (
                    ExampleUrl,
                    new []
                    {
                        new KeyValuePair<string, ICollection<string>>("id", new[] {"1"}),
                        new KeyValuePair<string, ICollection<string>>("name", new[] {"link"}),
                        new KeyValuePair<string, ICollection<string>>("email", new[] {"link@gmail.com"}),
                    },
                    $"{ExampleUrl}?id=1&name=link&email=link%40gmail.com"),
                (
                    ExampleUrl,
                    new []
                    {
                        new KeyValuePair<string, ICollection<string>>("id", new[] {"1", "2", "3"}),
                        new KeyValuePair<string, ICollection<string>>("name", new[] {"link"}),
                    },
                    $"{ExampleUrl}?id=1&id=2&id=3&name=link"),
                (
                    ExampleUrl,
                    new []
                    {
                        new KeyValuePair<string, ICollection<string>>("id", new[] {"1", "2", "3"}),
                        new KeyValuePair<string, ICollection<string>>("name", new[] {"link", "summer"}),
                        new KeyValuePair<string, ICollection<string>>("email", new[] {"link@gmail.com", "summer@gmail.com"}),
                    },
                    $"{ExampleUrl}?id=1&id=2&id=3&name=link&name=summer&email=link%40gmail.com&email=summer%40gmail.com")
            };
            var builder = IUrlBuilder.Current;
            foreach (var (url, parameters, expect) in table)
            {
                var result = builder.BuildUrl(url, parameters);
                Assert.AreEqual(expect, result);
            }
        }
        public class TestRequest : BaseRequest<TestRequest>
        {
            public class TestRequestBody
            {
                public int A { get; set; }
            }
            public TestRequestBody BodyData { get; set; }
            public DataFormat BodyFormat { get; set; }

            public override string SerializedBody => BodyData is null ? null : IDataSerializer.Current.Serialize(BodyData, BodyFormat);

            public TestRequest() : base(ExampleUrl, ExampleUrl)
            {
                Init();
            }
        }
        [Test]
        public void Test_Url_Should_Be_Rebuilt_When_Change_BaseUrl()
        {
            var request = new TestRequest
            {
                Url = ExampleUrl1
            };
            Assert.AreEqual(request.AbsoluteUrl, ExampleUrl1);
        }
        [Test]
        public void Test_Url_Should_Be_Rebuilt_When_Change_Param()
        {
            var request = new TestRequest();
            request.Params.AddParam("id", "1");
            Assert.AreEqual(request.AbsoluteUrl, $"{ExampleUrl}?id=1");
            request.Params.RemoveParam("id");
            Assert.AreEqual(request.AbsoluteUrl, ExampleUrl);
        }
        [Test]
        public void Test_Url_Should_Be_Rebuilt_When_Change_Param_List()
        {
            var request = new TestRequest();
            request.Params.AddParams("id", new []{"1", "2"});
            Assert.AreEqual(request.AbsoluteUrl, $"{ExampleUrl}?id=1&id=2");
            request.Params.RemoveValueFromParam("id", "2");
            Assert.AreEqual(request.AbsoluteUrl, $"{ExampleUrl}?id=1");
            request.Params.RemoveValueFromParam("id", "1");
            Assert.AreEqual(request.AbsoluteUrl, ExampleUrl);
        }
        
        [Test]
        public void Test_Json_Serialized_Body()
        {
            var request = new TestRequest();
            request.BodyData = new TestRequest.TestRequestBody
            {
                A = 5
            };
            Assert.AreEqual(request.SerializedBody, @"{""A"":5}");
            request.BodyData = new TestRequest.TestRequestBody
            {
                A = 3
            };
            Assert.AreEqual(request.SerializedBody, @"{""A"":3}");
        }
        
        [Test]
        public void Test_Xml_Serialized_Body()
        {
            var request = new TestRequest()
            {
                BodyFormat = DataFormat.Xml
            };
            request.BodyData = new TestRequest.TestRequestBody
            {
                A = 5
            };
            Assert.AreEqual(request.SerializedBody, "<root><A>5</A></root>");
            request.BodyData = new TestRequest.TestRequestBody
            {
                A = 3
            };
            Assert.AreEqual(request.SerializedBody, "<root><A>3</A></root>");
        }
        
        [Test]
        public void Test_Change_Serialized_Body_When_Change_Format()
        {
            var request = new TestRequest()
            {
                BodyFormat = DataFormat.Json
            };
            request.BodyData = new TestRequest.TestRequestBody
            {
                A = 5
            };
            Assert.AreEqual(request.SerializedBody, @"{""A"":5}");
            request.BodyFormat = DataFormat.Xml;
            request.BodyData = new TestRequest.TestRequestBody
            {
                A = 3
            };
            Assert.AreEqual(request.SerializedBody, "<root><A>3</A></root>");
        }
    }
}