using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.Request;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace SummerRest.Scripts.Tests
{
    public class InternalRequestsTest
    {
        [UnityTest]
        public IEnumerator Test_Internal_Request_Return_200_And_Json_Data()
        {
            var provider = new TestWebRequestAdaptorProvider("application/json", @"{""A"":5}", 200, null);
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestRequest.Create();
            var expected = new TestRequest.TestResponseData
            {
                A = 5
            };
            yield return request.SimpleResponseCoroutine<TestRequest.TestResponseData>(e =>
            {
                Assert.That(e.Equals(expected));
            });
            yield return request.SimpleResponseCoroutine<TestRequest.TestResponseData>(UnityWebRequest.Get(string.Empty), e =>
            {
                Assert.That(e.Equals(expected));
            });
            yield return request.DetailedResponseCoroutine<TestRequest.TestResponseData>(e =>
            {
                Assert.AreEqual(e.StatusCode, HttpStatusCode.OK);
                Assert.IsNull(e.Error);
                Assert.That(e.Data.Equals(expected));
            });
            yield return request.DetailedResponseCoroutine<TestRequest.TestResponseData>(UnityWebRequest.Get(string.Empty),e =>
            {
                Assert.AreEqual(e.StatusCode, HttpStatusCode.OK);
                Assert.IsNull(e.Error);
                Assert.That(e.Data.Equals(expected));
            });
        }
        [UnityTest]
        public IEnumerator Test_Internal_Request_Return_201_And_Xml_Data()
        {
            var provider = new TestWebRequestAdaptorProvider("application/xml", "<root><A>3</A></root>", 201, null);
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestRequest.Create();
            var expected = new TestRequest.TestResponseData
            {
                A = 3
            };
            yield return request.SimpleResponseCoroutine<TestRequest.TestResponseData>(e =>
            {
                Assert.That(e.Equals(expected));
            });
            yield return request.SimpleResponseCoroutine<TestRequest.TestResponseData>(UnityWebRequest.Get(string.Empty), e =>
            {
                Assert.That(e.Equals(expected));
            });
            yield return request.DetailedResponseCoroutine<TestRequest.TestResponseData>(e =>
            {
                Assert.AreEqual(e.StatusCode, HttpStatusCode.Created);
                Assert.IsNull(e.Error);
                Assert.That(e.Data.Equals(expected));
            });
            yield return request.DetailedResponseCoroutine<TestRequest.TestResponseData>(UnityWebRequest.Get(string.Empty),e =>
            {
                Assert.AreEqual(e.StatusCode, HttpStatusCode.Created);
                Assert.IsNull(e.Error);
                Assert.That(e.Data.Equals(expected));
            });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Return_500_And_Plain()
        {
            const string result = "internal server error response";
            var provider = new TestWebRequestAdaptorProvider("text/plain", result, 500, null);
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestRequest.Create();
            yield return request.SimpleResponseCoroutine<string>(e =>
            {
                Assert.AreEqual(result, e);
            });
            yield return request.SimpleResponseCoroutine<string>(UnityWebRequest.Get(string.Empty), e =>
            {
                Assert.AreEqual(result, e);
            });
            yield return request.DetailedResponseCoroutine<string>(e =>
            {
                Assert.AreEqual(e.StatusCode, HttpStatusCode.InternalServerError);
                Assert.IsNull(e.Error);
                Assert.AreEqual(result, e.Data);
            });
            yield return request.DetailedResponseCoroutine<string>(UnityWebRequest.Get(string.Empty),e =>
            {
                Assert.AreEqual(e.StatusCode, HttpStatusCode.InternalServerError);
                Assert.IsNull(e.Error);
                Assert.AreEqual(result, e.Data);
            });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_On_Error_Callback()
        {
            const string error = "connection error";
            var provider = new TestWebRequestAdaptorProvider("text/plain", string.Empty, 500, error);
            IWebRequestAdaptorProvider.Current = provider;
            yield return TestRequest.Create().SimpleResponseCoroutine<string>(null, e =>
            {
                Assert.AreEqual(error, e);
            });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Missing_Log_When_Error_Callback_Absent()
        {
            const string error = "connection error";
            var provider = new TestWebRequestAdaptorProvider("text/plain", string.Empty, 500, error);
            IWebRequestAdaptorProvider.Current = provider;
            yield return TestRequest.Create().SimpleResponseCoroutine<string>(null);
            var msg = string.Format(
                @"There was an missed error ""{0}"" when trying to access the resource {1}. Please give errorCallback to catch it",
                error, string.Empty);
            LogAssert.Expect(LogType.Error, msg);
        }
        public class TestRequest : BaseRequest<TestRequest>
        {
            public TestRequest() : base(string.Empty)
            {
                Init();
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
        }
        public class TestWebRequestAdaptorProvider : IWebRequestAdaptorProvider
        {
            private readonly string _fixedContentType;
            private readonly string _fixedRawResponse;
            private readonly int _code;
            private readonly string _fixedError;
            private readonly IWebRequestAdaptorProvider _wrapped;
            public TestWebRequestAdaptorProvider(string fixedContentType, string fixedRawResponse, int code,
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

            public IWebRequestAdaptor<TResponse> GetFromUnityWebRequest<TResponse>(UnityWebRequest webRequest)
            {
                return new RawTestWebRequestAdaptor<TResponse>(RawUnityWebRequestAdaptor<TResponse>.Create(webRequest),
                    _fixedContentType, _fixedRawResponse, _code, _fixedError);
            }

            public IWebRequestAdaptor<TBody> GetDataRequest<TBody>(string url, HttpMethod method, string bodyData)
            {
                var r = _wrapped.GetDataRequest<TBody>(url, method, bodyData) as RawUnityWebRequestAdaptor<TBody>;
                return new RawTestWebRequestAdaptor<TBody>(r, _fixedContentType, _fixedRawResponse, _code, _fixedError);
            }
        }
        public class RawTestWebRequestAdaptor<TResponse> : TestWebRequestAdaptor<
            RawUnityWebRequestAdaptor<TResponse>, TResponse>
        {
            public RawTestWebRequestAdaptor(RawUnityWebRequestAdaptor<TResponse> webRequest,
                string fixedContentType, string fixedRawResponse, int code, string fixedError) :
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

        public abstract class TestWebRequestAdaptor<TWrappedAdaptor, TResponse> : IWebRequestAdaptor<TResponse>
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

            public ContentType ContentType
            {
                get => Wrapped.ContentType;
                set => Wrapped.ContentType = value;
            }

            protected readonly TWrappedAdaptor Wrapped;
            protected readonly int StatusCode;
            protected readonly string FixedContentType;
            protected readonly string FixedRawResponse;
            protected readonly string FixedError;

            public TestWebRequestAdaptor(TWrappedAdaptor wrapped, string fixedContentType, string fixedRawResponse,
                int code, string fixedError)
            {
                Wrapped = wrapped;
                FixedContentType = fixedContentType;
                FixedRawResponse = fixedRawResponse;
                StatusCode = code;
                FixedError = fixedError;
            }

            public IWebResponse<TResponse> WebResponse =>
                new UnityWebResponse(StatusCode, FixedContentType, FixedRawResponse, ResponseData, FixedError);
            private readonly struct UnityWebResponse : IWebResponse<TResponse>
            {
                public object WrappedRequest => null;

                public UnityWebResponse(int code, string contentType, string rawResponse,
                    TResponse response, string error)
                {
                    RawData = rawResponse;
                    StatusCode = (HttpStatusCode)code;
                    _contentTypeStr = contentType;
                    Error = error;
                    Data = response;
                }
                public string RawData { get; }
                public IEnumerable<KeyValuePair<string, string>> Headers => null;
                private readonly string _contentTypeStr;

                public ContentType ContentType
                    => IContentTypeParser.Current.ParseContentTypeFromHeader(_contentTypeStr);
                public HttpStatusCode StatusCode { get; }
                public string Error { get; }
                public TResponse Data { get; }
            }
            public TResponse ResponseData { get; protected set; }
            public Task<TResponse> RequestAsync { get; }
        }
    }
}