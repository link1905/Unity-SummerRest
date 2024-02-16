using System;
using System.Collections.Generic;
using NUnit.Framework;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;

namespace SummerRest.Tests
{
    public class RequestBuilderTest
    {
        private const string ExampleUrl = "summerrest.com";
        private const string ExampleUrlFormat = "summerrest.com/{0}";
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
        [Serializable]
        public class TestRequestBody
        {
            public int A;
        }
        public class TestRequest : BaseDataRequest<TestRequest>
        {
            public TestRequest() : base(ExampleUrl, ExampleUrl, ExampleUrlFormat, new []{string.Empty}, null)
            {
                Init();
            }
        }
        [Test]
        public void Test_Url_Should_Be_Rebuilt_When_Change_FormatValues()
        {
            var request = new TestRequest();
            request.SetUrlValue(0, "my-id");
            Assert.AreEqual(request.AbsoluteUrl, string.Format(ExampleUrlFormat, "my-id"));
        }
        [Test]
        public void Test_Url_Should_Be_Rebuilt_When_Change_Param()
        {
            var request = new TestRequest();
            request.Params.AddParamToList("id", "1");
            Assert.AreEqual(request.AbsoluteUrl, $"{ExampleUrl}?id=1");
            request.Params.RemoveParam("id");
            Assert.AreEqual(request.AbsoluteUrl, ExampleUrl);
        }
        [Test]
        public void Test_Url_Should_Be_Rebuilt_When_Change_Param_List()
        {
            var request = new TestRequest();
            request.Params.AddParamToList("id", new []{"1", "2"});
            Assert.AreEqual(request.AbsoluteUrl, $"{ExampleUrl}?id=1&id=2");
            request.Params.RemoveValueFromList("id", "2");
            Assert.AreEqual(request.AbsoluteUrl, $"{ExampleUrl}?id=1");
            request.Params.RemoveValueFromList("id", "1");
            Assert.AreEqual(request.AbsoluteUrl, ExampleUrl);
        }
        
        [Test]
        public void Test_Json_Serialized_Body()
        {
            var request = new TestRequest();
            request.BodyData = new TestRequestBody
            {
                A = 5
            };
            Assert.AreEqual(request.SerializedBody, @"{""A"":5}");
            request.BodyData = new TestRequestBody
            {
                A = 3
            };
            Assert.AreEqual(request.SerializedBody, @"{""A"":3}");
        }
        
        [Test]
        public void Test_Xml_Serialized_Body()
        {
            var request = new TestRequest
            {
                BodyFormat = DataFormat.Xml,
                BodyData = new TestRequestBody
                {
                    A = 5
                }
            };
            Assert.AreEqual(@"<TestRequestBody xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><A>5</A></TestRequestBody>", request.SerializedBody);
            request.BodyData = new TestRequestBody
            {
                A = 3
            };
            Assert.AreEqual(@"<TestRequestBody xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><A>3</A></TestRequestBody>", request.SerializedBody);
        }
        
        [Test]
        public void Test_Change_Serialized_Body_When_Change_Format()
        {
            var request = new TestRequest()
            {
                BodyFormat = DataFormat.Json
            };
            request.BodyData = new TestRequestBody
            {
                A = 5
            };
            Assert.AreEqual(request.SerializedBody, @"{""A"":5}");
            request.BodyFormat = DataFormat.Xml;
            request.BodyData = new TestRequestBody
            {
                A = 3
            };
            Assert.AreEqual(request.SerializedBody, @"<TestRequestBody xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><A>3</A></TestRequestBody>");
        }
    }
}