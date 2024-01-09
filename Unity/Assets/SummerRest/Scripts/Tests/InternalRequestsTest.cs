using System.Collections;
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
            var provider = new TestWebRequestAdaptorProvider("application/json", @"{""A"":5}", HttpStatusCode.OK, null);
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestRequest.Create();
            var expected = new TestRequest.TestResponseData
            {
                A = 5
            };
            yield return request.SimpleRequestCoroutine<TestRequest.TestResponseData>(e =>
            {
                Assert.That(e.Equals(expected));
            });
            // yield return request.SimpleRequestCoroutine<TestRequest.TestResponseData>(UnityWebRequest.Get(string.Empty), e =>
            // {
            //     Assert.That(e.Equals(expected));
            // });
            yield return request.DetailedRequestCoroutine<TestRequest.TestResponseData>(e =>
            {
                Assert.AreEqual(HttpStatusCode.OK, e.StatusCode);
                Assert.IsNull(e.Error);
                Assert.That(e.Data.Equals(expected));
            });
            // yield return request.DetailedRequestCoroutine<TestRequest.TestResponseData>(UnityWebRequest.Get(string.Empty),e =>
            // {
            //     Assert.AreEqual(HttpStatusCode.OK, e.StatusCode);
            //     Assert.IsNull(e.Error);
            //     Assert.That(e.Data.Equals(expected));
            // });
        }
        [UnityTest]
        public IEnumerator Test_Internal_Request_Return_201_And_Xml_Data()
        {
            var provider = new TestWebRequestAdaptorProvider("application/xml", "<root><A>3</A></root>", HttpStatusCode.Created, null);
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestRequest.Create();
            var expected = new TestRequest.TestResponseData
            {
                A = 3
            };
            yield return request.SimpleRequestCoroutine<TestRequest.TestResponseData>(e =>
            {
                Assert.That(e.Equals(expected));
            });
            // yield return request.SimpleRequestCoroutine<TestRequest.TestResponseData>(UnityWebRequest.Get(string.Empty), e =>
            // {
            //     Assert.That(e.Equals(expected));
            // });
            yield return request.DetailedRequestCoroutine<TestRequest.TestResponseData>(e =>
            {
                Assert.AreEqual(HttpStatusCode.Created, e.StatusCode);
                Assert.IsNull(e.Error);
                Assert.That(e.Data.Equals(expected));
            });
            // yield return request.DetailedAudioRequestCoroutine<TestRequest.TestResponseData>(UnityWebRequest.Get(string.Empty),e =>
            // {
            //     Assert.AreEqual(HttpStatusCode.Created, e.StatusCode);
            //     Assert.IsNull(e.Error);
            //     Assert.That(e.Data.Equals(expected));
            // });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Return_500_And_Plain()
        {
            const string result = "internal server error response";
            var provider = new TestWebRequestAdaptorProvider("text/plain", result, HttpStatusCode.InternalServerError, null);
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestRequest.Create();
            yield return request.SimpleRequestCoroutine<string>(e =>
            {
                Assert.AreEqual(result, e);
            });
            // yield return request.SimpleRequestCoroutine<string>(UnityWebRequest.Get(string.Empty), e =>
            // {
            //     Assert.AreEqual(result, e);
            // });
            yield return request.DetailedRequestCoroutine<string>(e =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, e.StatusCode);
                Assert.IsNull(e.Error);
                Assert.AreEqual(result, e.Data);
            });
            // yield return request.DetailedAudioRequestCoroutine<string>(UnityWebRequest.Get(string.Empty),e =>
            // {
            //     Assert.AreEqual(e.StatusCode, HttpStatusCode.InternalServerError);
            //     Assert.IsNull(e.Error);
            //     Assert.AreEqual(result, e.Data);
            // });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_On_Error_Callback()
        {
            const string error = "connection error";
            var provider = new TestWebRequestAdaptorProvider("text/plain", string.Empty, HttpStatusCode.InternalServerError, error);
            IWebRequestAdaptorProvider.Current = provider;
            yield return TestRequest.Create().SimpleRequestCoroutine<string>(null, e =>
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
            yield return TestRequest.Create().SimpleRequestCoroutine<string>(null);
            var msg = string.Format(
                @"There was an missed error ""{0}"" when trying to access the resource {1}. Please give errorCallback to catch it",
                error, string.Empty);
            LogAssert.Expect(LogType.Error, msg);
        }
        public class TestRequest : BaseRequest<TestRequest>
        {
            public TestRequest() : base(string.Empty, string.Empty)
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

            public TestWebRequestAdaptor(TWrappedAdaptor wrapped, string fixedContentType, string fixedRawResponse,
                HttpStatusCode code, string fixedError)
            {
                Wrapped = wrapped;
                FixedContentType = fixedContentType;
                FixedRawResponse = fixedRawResponse;
                StatusCode = code;
                FixedError = fixedError;
            }

            public WebResponse<TResponse> WebResponse             => new(null,
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