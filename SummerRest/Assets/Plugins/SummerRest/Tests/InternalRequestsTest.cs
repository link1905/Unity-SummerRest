using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NUnit.Framework;
using SummerRest.Runtime.Authenticate.Appenders;
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
            var provider = new TestWebRequestAdaptorProvider()
            {
                FixedContentType = "application/json",
                FixedRawResponse = @"{""A"":5}",
                Code = HttpStatusCode.OK
            };
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestDataRequest.Create();
            var expected = new TestResponseData
            {
                A = 5
            };
            yield return request.DataRequestCoroutine<TestResponseData>(e =>
            {
                Assert.That(e.Equals(expected));
            });
            yield return request.DetailedDataRequestCoroutine<TestResponseData>(e =>
            {
                Assert.AreEqual(HttpStatusCode.OK, e.StatusCode);
                Assert.IsNull(e.Error);
                Assert.That(e.Data.Equals(expected));
            });
        }
        [UnityTest]
        public IEnumerator Test_Internal_Request_Return_201_And_Xml_Data()
        {
            var provider = new TestWebRequestAdaptorProvider()
            {
                FixedContentType = "application/xml",
                FixedRawResponse = "<root><A>3</A></root>",
                Code = HttpStatusCode.Created
            };
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestDataRequest.Create();
            var expected = new TestResponseData
            {
                A = 3
            };
            yield return request.DataRequestCoroutine<TestResponseData>(e =>
            {
                Assert.That(e.Equals(expected));
            });
            yield return request.DetailedDataRequestCoroutine<TestResponseData>(e =>
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
            var provider = new TestWebRequestAdaptorProvider()            
            {
                FixedContentType = "text/plain",
                FixedRawResponse = result,
                Code = HttpStatusCode.InternalServerError
            };
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestDataRequest.Create();
            yield return request.DataRequestCoroutine<string>(e =>
            {
                Assert.AreEqual(result, e);
            });
            yield return request.DetailedDataRequestCoroutine<string>(e =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, e.StatusCode);
                Assert.IsNull(e.Error);
                Assert.AreEqual(result, e.Data);
            });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Texture_Request_Return_Correct_Texture()
        {
            var result = new Texture2D(20, 20);
            var provider = new TestWebRequestAdaptorProvider
            {
                Code = HttpStatusCode.OK,
                ResponseTexture = result
            };
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestDataRequest.Create();
            yield return request.TextureRequestCoroutine(false, e =>
            {
                Assert.AreSame(result, e);
            });
            yield return request.DetailedTextureRequestCoroutine(false, e =>
            {
                Assert.AreSame(result, e.Data);
                Assert.AreEqual(HttpStatusCode.OK, e.StatusCode);
            });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Audio_Request_Return_Correct_Audio()
        {
            var result = AudioClip.Create("test-clip", 1, 1, 1, false);
            var provider = new TestWebRequestAdaptorProvider
            {
                Code = HttpStatusCode.OK,
                ResponseAudioClip = result
            };
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestDataRequest.Create();
            yield return request.AudioRequestCoroutine(AudioType.WAV, e =>
            {
                Assert.AreSame(result, e);
            });
            yield return request.DetailedAudioRequestCoroutine(AudioType.WAV, e =>
            {
                Assert.AreSame(result, e.Data);
                Assert.AreEqual(HttpStatusCode.OK, e.StatusCode);
            });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_On_Error_Callback()
        {
            const string error = "connection error";
            var provider = new TestWebRequestAdaptorProvider
            {
                FixedError = error,
                Code = HttpStatusCode.InternalServerError
            };
            IWebRequestAdaptorProvider.Current = provider;
            yield return TestDataRequest.Create().DataRequestCoroutine<string>(null, e =>
            {
                Assert.AreEqual(error, e.Message);
                Assert.AreEqual(HttpStatusCode.InternalServerError, e.StatusCode);
            });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Missing_Log_When_Error_Callback_Absent()
        {
            const string error = "connection error";
            var provider = new TestWebRequestAdaptorProvider()            
            {
                FixedError = error,
                Code = HttpStatusCode.InternalServerError
            };
            IWebRequestAdaptorProvider.Current = provider;
            yield return TestDataRequest.Create().DataRequestCoroutine<string>(null);
            var msg = string.Format(
                @"There was an missed error ""{0}"" when trying to access the resource {1}. Please give errorCallback to catch it",
                error, TestAbsoluteUrl);
            LogAssert.Expect(LogType.Error, msg);
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Append_Bearer_Header()
        {
            var provider = new TestWebRequestAdaptorProvider();
            IWebRequestAdaptorProvider.Current = provider;
            ISecretRepository.Current.Save("test-key", "my-token");
            var request = TestDataRequest.Create();
            request.AuthKey = "test-key";
            string header = null;
            yield return request.DetailedDataRequestCoroutine<string>(s =>
            {
                header = ((UnityWebRequest)s.WrappedRequest).GetRequestHeader("Authorization");
            });
            Assert.AreEqual("Bearer my-token", header);
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Append_Correct_Multipart_Form()
        {
            var contentType = ContentType.Commons.MultipartForm;
            var form = new List<IMultipartFormSection>();
            form.Add(new MultipartFormDataSection("form-key-1", "form-value-1"));
            form.Add(new MultipartFormDataSection("form-key-2", "form-value-2"));
            
            var provider = new TestWebRequestAdaptorProvider()
            {
                FixedContentType = contentType.FormedContentType,
            };
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestMultipartRequest.Create();
            byte[] data = null;
            yield return request.DetailedMultipartDataRequestCoroutine<string>(e =>
            {
                data = ((UnityWebRequest)e.WrappedRequest).uploadHandler.data;
            });
            CollectionAssert.AreEqual(UnityWebRequest.SerializeFormSections(form, Encoding.UTF8.GetBytes(contentType.Boundary)),
                data);
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
            public TestDataRequest() : base(TestUrl, TestAbsoluteUrl, null, null,
                new AuthRequestModifier<BearerTokenAuthAppender, string>())
            {
                Init();
            }
        }
        
        public class TestMultipartRequest : BaseMultipartRequest<TestMultipartRequest>
        {
            public TestMultipartRequest() : base(TestUrl, TestAbsoluteUrl, null, null, null)
            {
                Init();
            }
        }

        private class TestWebRequestAdaptorProvider : IWebRequestAdaptorProvider
        {
            public string FixedContentType { get; set; }
            public string FixedRawResponse { get; set; }
            public HttpStatusCode Code { get; set; }
            public string FixedError { get; set; }
            public Texture2D ResponseTexture { get; set; }
            public AudioClip ResponseAudioClip { get; set; }
            private readonly IWebRequestAdaptorProvider _wrapped;
   

            public IWebRequestAdaptor<Texture2D> GetTextureRequest(string url, bool nonReadable)
            {
                var wrapped = _wrapped.GetTextureRequest(url, nonReadable) as TextureUnityWebRequestAdaptor;
                return new TextureTestWebRequestAdaptor(ResponseTexture, wrapped, FixedContentType, FixedRawResponse, Code, FixedError);
            }

            public IWebRequestAdaptor<AudioClip> GetAudioRequest(string url, AudioType audioType)
            {
                var wrapped = _wrapped.GetAudioRequest(url, audioType) as AudioUnityWebRequestAdaptor;
                return new AudioTestWebRequestAdaptor(ResponseAudioClip, wrapped, FixedContentType, FixedRawResponse, Code, FixedError);
            }
            public IWebRequestAdaptor<TBody> GetMultipartFileRequest<TBody>(string url, HttpMethod method, List<IMultipartFormSection> data)
            {
                var wrapped = _wrapped.GetMultipartFileRequest<TBody>(url, method, data) as MultipartFileUnityWebRequestAdaptor<TBody>;
                return new MultipartTestWebRequestAdaptor<TBody>(wrapped, FixedContentType, FixedRawResponse, Code, FixedError);
            }


            public IWebRequestAdaptor<TBody> GetDataRequest<TBody>(string url, HttpMethod method, string bodyData, string contentType)
            {
                var r = _wrapped.GetDataRequest<TBody>(url, method, bodyData, contentType) as RawUnityWebRequestAdaptor<TBody>;
                return new RawTestWebRequestAdaptor<TBody>(r, FixedContentType, FixedRawResponse, Code, FixedError);
            }


        }

        private class MultipartTestWebRequestAdaptor<TResponse> : RawTestWebRequestAdaptor<TResponse>
        {
            public MultipartTestWebRequestAdaptor(MultipartFileUnityWebRequestAdaptor<TResponse> wrapped, string fixedContentType, 
                string fixedRawResponse, HttpStatusCode code, string fixedError) : base(wrapped, fixedContentType, fixedRawResponse, code, fixedError)
            {
            }
        }
        
        private class TextureTestWebRequestAdaptor : BaseTestWebRequestAdaptor<
            TextureUnityWebRequestAdaptor, Texture2D>
        {
            private readonly Texture2D _result;
            public TextureTestWebRequestAdaptor(Texture2D result, TextureUnityWebRequestAdaptor wrapped, string fixedContentType, string fixedRawResponse, HttpStatusCode code, string fixedError) 
                : base(wrapped, fixedContentType, fixedRawResponse, code, fixedError)
            {
                _result = result;
            }
            public override IEnumerator RequestInstruction
            {
                get
                {
                    yield return null;
                    ResponseData = _result;
                }
            }
        }
        
        private class AudioTestWebRequestAdaptor : BaseTestWebRequestAdaptor<
            AudioUnityWebRequestAdaptor, AudioClip>
        {
            private readonly AudioClip _result;
            public AudioTestWebRequestAdaptor(AudioClip result, AudioUnityWebRequestAdaptor wrapped, string fixedContentType, string fixedRawResponse, HttpStatusCode code, string fixedError) 
                : base(wrapped, fixedContentType, fixedRawResponse, code, fixedError)
            {
                _result = result;
            }
            public override IEnumerator RequestInstruction
            {
                get
                {
                    yield return null;
                    ResponseData = _result;
                }
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

            public bool IsError(out ResponseError error)
            {
                if (string.IsNullOrEmpty(FixedError))
                {
                    error = default;
                    return false;
                }
                error = new ResponseError(FixedError, FixedRawResponse, StatusCode);
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