using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NUnit.Framework;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Extensions;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;
using ISecretRepository = SummerRest.Runtime.Authenticate.Repositories.ISecretRepository;

namespace SummerRest.Tests
{
    public class InternalRequestsTest
    {
        private const string TestUrl = "http://example.com/";
        private const string TestAbsoluteUrl = "http://example.com/200";
        [UnityTest]
        public IEnumerator Test_Internal_Request_Return_200_And_Json_Data()
        {
            var provider = new TestWebRequestAdaptorProvider("application/json", @"{""A"":5}", HttpStatusCode.OK, null);
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestDataRequest.Create();
            var expected = new TestResponseData
            {
                A = 5
            };
            yield return request.RequestCoroutine<TestResponseData>(e =>
            {
                Assert.That(e.Equals(expected));
            });
            yield return request.DetailedRequestCoroutine<TestResponseData>(e =>
            {
                Assert.AreEqual(HttpStatusCode.OK, e.StatusCode);
                Assert.IsNull(e.Error);
                Assert.That(e.Data.Equals(expected));
            });
        }
        [UnityTest]
        public IEnumerator Test_Internal_Request_Return_201_And_Xml_Data()
        {
            var provider = new TestWebRequestAdaptorProvider("application/xml", "<root><A>3</A></root>", HttpStatusCode.Created, null);
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestDataRequest.Create();
            var expected = new TestResponseData
            {
                A = 3
            };
            yield return request.RequestCoroutine<TestResponseData>(e =>
            {
                Assert.That(e.Equals(expected));
            });
            yield return request.DetailedRequestCoroutine<TestResponseData>(e =>
            {
                Assert.AreEqual(HttpStatusCode.Created, e.StatusCode);
                Assert.IsNull(e.Error);
                Assert.That(e.Data.Equals(expected));
            });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Return_500_And_Plain()
        {
            const string result = "internal server error response";
            var provider = new TestWebRequestAdaptorProvider("text/plain", result, HttpStatusCode.InternalServerError, null);
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestDataRequest.Create();
            yield return request.RequestCoroutine<string>(e =>
            {
                Assert.AreEqual(result, e);
            });
            yield return request.DetailedRequestCoroutine<string>(e =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, e.StatusCode);
                Assert.IsNull(e.Error);
                Assert.AreEqual(result, e.Data);
            });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_On_Error_Callback()
        {
            const string error = "connection error";
            var provider = new TestWebRequestAdaptorProvider("text/plain", string.Empty, HttpStatusCode.InternalServerError, error);
            IWebRequestAdaptorProvider.Current = provider;
            yield return TestDataRequest.Create().RequestCoroutine<string>(null, e =>
            {
                Assert.AreEqual(error, e);
            });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Missing_Log_When_Error_Callback_Absent()
        {
            const string error = "connection error";
            var provider = new TestWebRequestAdaptorProvider("text/plain", string.Empty, HttpStatusCode.InternalServerError, error);
            IWebRequestAdaptorProvider.Current = provider;
            yield return TestDataRequest.Create().RequestCoroutine<string>(null);
            var msg = string.Format(
                @"There was an missed error ""{0}"" when trying to access the resource {1}. Please give errorCallback to catch it",
                error, TestAbsoluteUrl);
            LogAssert.Expect(LogType.Error, msg);
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Append_Bearer_Header()
        {
            var provider = new TestWebRequestAdaptorProvider("text/plain", string.Empty, HttpStatusCode.OK, null);
            IWebRequestAdaptorProvider.Current = provider;
            ISecretRepository.Current.Save("test-key", "my-token");
            var request = TestDataRequest.Create();
            request.AuthKey = "test-key";
            yield return request.TestAuthRequestCoroutine<string, BearerTokenAuthAppender, string>(null);
            var header = ((RawTestWebRequestAdaptor<string>)request.PreviousRequest).GetHeader("Authorization");
            Assert.AreEqual("Bearer my-token", header);
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Append_Correct_Multipart_Form()
        {
            var contentType = ContentType.Commons.MultipartForm;
            var form = new List<IMultipartFormSection>();
            form.Add(new MultipartFormDataSection("form-key-1", "form-value-1"));
            form.Add(new MultipartFormDataSection("form-key-2", "form-value-2"));
            
            var provider = new TestWebRequestAdaptorProvider(contentType.FormedContentType, string.Empty, HttpStatusCode.OK, null);
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestMultipartRequest.Create();
            yield return request.TestRequestCoroutine<string>(null);
            CollectionAssert.AreEqual(UnityWebRequest.SerializeFormSections(form, Encoding.UTF8.GetBytes(contentType.Boundary)),
                ((MultipartTestWebRequestAdaptor<string>)request.PreviousRequest).Data);
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_From_Unity_Web_Request_Set_Correct_Information()
        {
            var provider = new TestWebRequestAdaptorProvider("text/plain", string.Empty, HttpStatusCode.OK, null);
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestDataRequest.Create();
            request.Method = HttpMethod.Connect;
            var webRequest = UnityWebRequest.Get(string.Empty);
            yield return request.RequestCoroutineFromUnityWebRequest(webRequest, e =>
            {
                Assert.AreSame(webRequest, e);
                Assert.AreEqual(TestAbsoluteUrl, e.url);
                Assert.AreEqual(HttpMethod.Connect.ToUnityHttpMethod(), e.method);
            });
            
            request.Method = HttpMethod.Get;
            request.Params.AddParamToList("my-param", "my-param-value");
            request.Headers.Add("my-header", "my-header-value");
            yield return request.DetailedRequestCoroutineFromUnityWebRequest(webRequest, e =>
            {
                Assert.IsNull(e.Error);
                Assert.AreEqual(HttpMethod.Get.ToUnityHttpMethod(), e.Data.method);
                Assert.That(e.Data.url.Contains("my-param=my-param-value"));
                Assert.AreEqual("my-header-value", e.Data.GetRequestHeader("my-header"));
                
            });
        }
        
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
        
        //Proxying classes for testing only
        public class TestDataRequest : BaseDataRequest<TestDataRequest>
        {
            public TestDataRequest() : base(TestUrl, TestAbsoluteUrl, 
                new AuthRequestModifier<BearerTokenAuthAppender, string>())
            {
                Init();
            }
            public object PreviousRequest { get; private set; } 
            public IEnumerator TestAuthRequestCoroutine<TResponse, TAuthAppender, TAuthData>(Action<TResponse> doneCallback, Action<string> errorCallback = null) 
                where TAuthAppender : class, IAuthAppender<TAuthAppender, TAuthData>, new()
            {
                var request =
                    IWebRequestAdaptorProvider.Current.GetDataRequest<TResponse>(AbsoluteUrl, Method, SerializedBody);
                PreviousRequest = request;
                yield return RequestCoroutine(request, doneCallback, errorCallback);
            }
        }
        
        public class TestMultipartRequest : BaseMultipartRequest<TestMultipartRequest>
        {
            public TestMultipartRequest() : base(TestUrl, TestAbsoluteUrl, null)
            {
                Init();
            }
            public object PreviousRequest { get; private set; } 
            public IEnumerator TestRequestCoroutine<TResponse>(Action<TResponse> doneCallback, Action<string> errorCallback = null)
            {
                var request = RequestCoroutine(doneCallback, errorCallback);
                PreviousRequest = request;
                yield return request;
            }
        }

        private class TestWebRequestAdaptorProvider : IWebRequestAdaptorProvider
        {
            private readonly string _fixedContentType;
            private readonly string _fixedRawResponse;
            private readonly HttpStatusCode _code;
            private readonly string _fixedError;
            private readonly IWebRequestAdaptorProvider _wrapped;
            public TestWebRequestAdaptorProvider(string fixedContentType, string fixedRawResponse, HttpStatusCode code,
                string fixedError)
            {
                this._fixedContentType = fixedContentType;
                this._fixedRawResponse = fixedRawResponse;
                this._code = code;
                this._fixedError = fixedError;
                _wrapped = new UnityWebRequestAdaptorProvider();
            }

            public IWebRequestAdaptor<Texture2D> GetTextureRequest(string url, bool readable)
            {
                return _wrapped.GetTextureRequest(url, readable);
            }

            public IWebRequestAdaptor<AudioClip> GetAudioRequest(string url, AudioType audioType)
            {
                return _wrapped.GetAudioRequest(url, audioType);
            }
            public IWebRequestAdaptor<TBody> GetMultipartFileRequest<TBody>(string url, List<IMultipartFormSection> data)
            {
                var wrapped = _wrapped.GetMultipartFileRequest<TBody>(url, data) as MultipartFileUnityWebRequestAdaptor<TBody>;
                return new MultipartTestWebRequestAdaptor<TBody>(wrapped, _fixedContentType, _fixedRawResponse, _code, _fixedError);
            }

            public IWebRequestAdaptor<UnityWebRequest> GetFromUnityWebRequest(UnityWebRequest webRequest)
            {
                var wrapped = _wrapped.GetFromUnityWebRequest(webRequest) as DumpUnityWebRequestAdaptor;
                return new DumpTestWebRequestAdaptor(wrapped, _fixedContentType, _fixedRawResponse, _code, _fixedError);
            }

            public IWebRequestAdaptor<TBody> GetDataRequest<TBody>(string url, HttpMethod method, string bodyData)
            {
                var r = _wrapped.GetDataRequest<TBody>(url, method, bodyData) as RawUnityWebRequestAdaptor<TBody>;
                return new RawTestWebRequestAdaptor<TBody>(r, _fixedContentType, _fixedRawResponse, _code, _fixedError);
            }


        }

        private class MultipartTestWebRequestAdaptor<TResponse> : RawTestWebRequestAdaptor<TResponse>
        {
            public byte[] Data => (Wrapped as MultipartFileUnityWebRequestAdaptor<TResponse>)?.Data;
            public MultipartTestWebRequestAdaptor(MultipartFileUnityWebRequestAdaptor<TResponse> wrapped, string fixedContentType, 
                string fixedRawResponse, HttpStatusCode code, string fixedError) : base(wrapped, fixedContentType, fixedRawResponse, code, fixedError)
            {
            }
        }
        private class DumpTestWebRequestAdaptor : BaseTestWebRequestAdaptor<DumpUnityWebRequestAdaptor, UnityWebRequest>
        {
            public DumpTestWebRequestAdaptor(DumpUnityWebRequestAdaptor wrapped, string fixedContentType, string fixedRawResponse, HttpStatusCode code, string fixedError) 
                : base(wrapped, fixedContentType, fixedRawResponse, code, fixedError)
            {
                
            }
            public override IEnumerator RequestInstruction
            {
                get
                {
                    yield return null;
                    ResponseData = Wrapped.BuildResponse();
                }
            }
        }

        private class RawTestWebRequestAdaptor<TResponse> : BaseTestWebRequestAdaptor<
            RawUnityWebRequestAdaptor<TResponse>, TResponse>
        {
            public RawTestWebRequestAdaptor(RawUnityWebRequestAdaptor<TResponse> webRequest,
                string fixedContentType, string fixedRawResponse, HttpStatusCode code, string fixedError) :
                base(webRequest, fixedContentType, fixedRawResponse, code, fixedError)
            {
            }
            public override IEnumerator RequestInstruction
            {
                get
                {
                    yield return null;
                    ResponseData = Wrapped.BuildResponse(FixedContentType, FixedRawResponse);
                }
            }
        }

        private abstract class BaseTestWebRequestAdaptor<TWrappedAdaptor, TResponse> : IWebRequestAdaptor<TResponse>
            where TWrappedAdaptor : UnityWebRequestAdaptor<TWrappedAdaptor, TResponse>, new()
        {
            public abstract IEnumerator RequestInstruction { get; }

            public void Dispose()
            {
                Wrapped.Dispose();
            }
            public string Url { get; set; }
            public void SetHeader(string key, string value)
            {
                Wrapped.SetHeader(key, value);
            }

            public bool IsError(out string error)
            {
                if (string.IsNullOrEmpty(FixedError))
                {
                    error = null;
                    return false;
                }
                error = FixedError;
                return true;
            }

            public string GetHeader(string key) => Wrapped.GetHeader(key);

            public HttpMethod Method
            {
                get => Wrapped.Method;
                set => Wrapped.Method = value;
            }

            public int RedirectLimit
            {
                get => Wrapped.RedirectLimit;
                set => Wrapped.RedirectLimit = value;
            }

            public int TimeoutSeconds
            {
                get => Wrapped.TimeoutSeconds;
                set => Wrapped.TimeoutSeconds = value;
            }

            public ContentType? ContentType
            {
                get => Wrapped.ContentType;
                set => Wrapped.ContentType = value;
            }

            protected readonly TWrappedAdaptor Wrapped;
            protected readonly HttpStatusCode StatusCode;
            protected readonly string FixedContentType;
            protected readonly string FixedRawResponse;
            protected readonly string FixedError;

            public BaseTestWebRequestAdaptor(TWrappedAdaptor wrapped, string fixedContentType, string fixedRawResponse,
                HttpStatusCode code, string fixedError)
            {
                Wrapped = wrapped;
                FixedContentType = fixedContentType;
                FixedRawResponse = fixedRawResponse;
                StatusCode = code;
                FixedError = fixedError;
            }

            public WebResponse<TResponse> WebResponse => new(null,
                StatusCode,
                IContentTypeParser.Current.ParseContentTypeFromHeader(FixedContentType),
                null,
                FixedError,
                FixedRawResponse ,
                ResponseData
            );
            public TResponse ResponseData { get; protected set; }
        }
    }
}