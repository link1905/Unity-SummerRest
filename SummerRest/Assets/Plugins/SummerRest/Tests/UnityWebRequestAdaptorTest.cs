using System;
using NUnit.Framework;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;
using UnityEngine.Networking;

namespace SummerRest.Tests
{
    public class UnityWebRequestAdaptorTest
    {
        public class TestResponseData
        {
            public int A { get; set; }
            public bool Equals(TestResponseData other)
            {
                return A == other.A;
            }

            public override int GetHashCode()
            {
                return A;
            }
        }

        private const string ExampleUrl = "https://example.com";
        private const string SummerUrl = "https://summerrest.com";
        public static UnityWebRequest WebRequest => UnityWebRequest.Get(ExampleUrl);
        public static UnityWebRequest PutWebRequest => UnityWebRequest.Put(ExampleUrl, string.Empty);
        [Test]
        public void Test_Url_Apply_After_Changing_Url_Property()
        {
            using var adaptor = RawUnityWebRequestAdaptor<TestResponseData>.Create(WebRequest);
            adaptor.Url = SummerUrl;
            var expected = new Uri(SummerUrl);
            Assert.AreEqual(expected.AbsoluteUri, adaptor.Url);
        }
        [Test]
        public void Test_Header_Apply_After_Add_Header()
        {
            using var adaptor = RawUnityWebRequestAdaptor<TestResponseData>.Create(WebRequest);
            adaptor.SetHeader("h1", "value1");
            adaptor.SetHeader("h2", "value2");
            Assert.AreEqual("value1", adaptor.GetHeader("h1"));
            Assert.AreEqual("value2", adaptor.GetHeader("h2"));
            Assert.IsNull(adaptor.GetHeader("h3"));
        }
        [Test]
        public void Test_Method_Apply_After_Set_Method()
        {
            using var adaptor = RawUnityWebRequestAdaptor<TestResponseData>.Create(WebRequest);
            Assert.AreEqual(HttpMethod.Get, adaptor.Method);
            adaptor.Method = HttpMethod.Post;
            Assert.AreEqual(HttpMethod.Post, adaptor.Method);
            adaptor.Method = HttpMethod.Put;
            Assert.AreEqual(HttpMethod.Put, adaptor.Method);
        }
        [Test]
        public void Test_Redirect_Apply_After_Set_RedirectLimit()
        {
            using var adaptor = RawUnityWebRequestAdaptor<TestResponseData>.Create(WebRequest);
            adaptor.RedirectLimit = 19;
            Assert.AreEqual(19, adaptor.RedirectLimit);
            adaptor.RedirectLimit = 5;
            Assert.AreEqual(5, adaptor.RedirectLimit);
        }
        [Test]
        public void Test_Timeout_Apply_After_Set_TimeoutSeconds()
        {
            using var adaptor = RawUnityWebRequestAdaptor<TestResponseData>.Create(WebRequest);
            adaptor.TimeoutSeconds = 1905;
            Assert.AreEqual(1905, adaptor.TimeoutSeconds);
            adaptor.TimeoutSeconds = 3330;
            Assert.AreEqual(3330, adaptor.TimeoutSeconds);
        }
        [Test]
        public void Test_Content_Type_Boundary_Is_Always_Set()
        {
            using var adaptor = MultipartFileUnityWebRequestAdaptor<TestResponseData>.Create(PutWebRequest);
            adaptor.ContentType = ContentType.Commons.Binary;
            Assert.IsNotEmpty(adaptor.ContentType.Value.FormedContentType);
        }

        [Test] public void Test_Default_Content_Type_Is_Null_When_Upload_Handler_Is_Null()
        {
            using var adaptor = RawUnityWebRequestAdaptor<TestResponseData>.Create(WebRequest);
            Assert.IsNull(adaptor.ContentType);
        }
        [Test]
        public void Test_Content_Type_Null_With_Get_Request()
        {
            using var adaptor = RawUnityWebRequestAdaptor<TestResponseData>.Create(WebRequest);
            Assert.IsNull(adaptor.ContentType);
            adaptor.ContentType = new ContentType();
            Assert.IsNull(adaptor.ContentType);
        }
        [Test]
        public void Test_Content_Type_Apply_After_Set()
        {
            using var adaptor = RawUnityWebRequestAdaptor<TestResponseData>.Create(PutWebRequest);
            var result = new ContentType("application/json", "utf-8", "www123");
            adaptor.ContentType = result; 
            Assert.That(adaptor.ContentType.Equals(result));
        }

        [Test]
        public void Test_Not_Empty_Boundary_With_Multipart_File_Request()
        {
            using var adaptor = MultipartFileUnityWebRequestAdaptor<TestResponseData>.Create(PutWebRequest);
            adaptor.ContentType = null;
            Assert.NotNull(adaptor.ContentType);
            Assert.IsNotEmpty(adaptor.ContentType.Value.Boundary);
            adaptor.ContentType = new ContentType();
            Assert.IsNotEmpty(adaptor.ContentType.Value.Boundary);
        }
        [Test]
        public void Test_Build_Json_Response()
        {
            using var adaptor = RawUnityWebRequestAdaptor<TestResponseData>.Create(WebRequest);
            var expected = new TestResponseData
            {
                A = 5
            };
            var response = adaptor.BuildResponse("application/json", @"{""A"":5}");
            Assert.That(expected.Equals(response));
        }
        [Test]
        public void Test_Build_Xml_Response()
        {
            using var adaptor = RawUnityWebRequestAdaptor<TestResponseData>.Create(WebRequest);
            var expected = new TestResponseData
            {
                A = 3
            };
            var response = adaptor.BuildResponse("application/xml", "<root><A>3</A></root>");
            Assert.That(expected.Equals(response));
        }
        private void Test_Build_Primitive_Response<TResponse>(string json, string xml, string plain, TResponse expected)
        {
            using var adaptor = RawUnityWebRequestAdaptor<TResponse>.Create(WebRequest);
            var response = adaptor.BuildResponse("application/json", json);
            Assert.AreEqual(expected, response);
            response = adaptor.BuildResponse("application/xml", xml);
            Assert.AreEqual(expected, response);
            response = adaptor.BuildResponse("text/plain", plain);
            Assert.AreEqual(expected, response);
        }
        [Test]
        public void Test_Build_Primitive_String_Response()
        {
            Test_Build_Primitive_Response("3", "<root>3</root>", "3", "3");
        }
        [Test]
        public void Test_Build_Primitive_Int_Response()
        {
            Test_Build_Primitive_Response("3", "<root>3</root>", "3", 3);
        }
        [Test]
        public void Test_Build_Primitive_Float_Response()
        {
            Test_Build_Primitive_Response("3.1", "<root>3.1</root>", "3.1", 3.1f);
        }
    }
}