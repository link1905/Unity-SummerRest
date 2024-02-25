using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Authenticate.Repositories;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace Tests.Runtime
{
    public class InternalRequestsTest
    {
        private const string TestUrl = "http://example.com/";
        private const string TestAbsoluteUrl = "http://example.com/200";
        private static readonly ISecretRepository SecretRepository = new PlayerPrefsSecretRepository(); 

        
        private static async UniTask TestSimpleAndDetailedRequestAsync<T>(
            T result,
            UniTask<T> simpleTask,
            UniTask<IWebResponse<T>> detailedTask,
            HttpStatusCode code = HttpStatusCode.OK)
        {
            var (data, response) = await UniTask.WhenAll(simpleTask, detailedTask);
            Assert.AreEqual(result, data);
            using (response)
            {
                Assert.AreEqual(result, response.Data);
                Assert.AreEqual(code, response.StatusCode);
            }
        }
        private static IEnumerator TestSimpleAndDetailedRequestCoroutine<T>(
            T result,
            Func<Action<T>, IEnumerator> simpleCor,
            Func<Action<IWebResponse<T>>, IEnumerator> detailedCor,
            HttpStatusCode code = HttpStatusCode.OK)
        {
            yield return simpleCor(data =>
            {
                Assert.AreEqual(result, data);
            });
            yield return detailedCor(response =>
            {
                using (response)
                {
                    Assert.AreEqual(code, response.StatusCode);
                    Assert.AreEqual(result, response.Data);
                }
            });
        }
        private static (TestDataRequest, TestResponseData) Setup_Test_Internal_Request_Return_200_And_Json_Data()
        {
            var provider = new TestWebRequestAdaptorProvider
            {
                FixedContentType = "application/json",
                FixedRawResponse = @"{""a"":5}",
                Code = HttpStatusCode.OK
            };
            IWebRequestAdaptorProvider.Current = provider;
            return (TestDataRequest.Create(), new TestResponseData
            {
                a = 5
            });
        }
        [UnityTest]
        public IEnumerator Test_Internal_Request_Return_200_And_Json_Data()
        {
            var (request, expected) = Setup_Test_Internal_Request_Return_200_And_Json_Data();
            yield return TestSimpleAndDetailedRequestCoroutine(expected, 
                c => request.DataRequestCoroutine(c),
                c => request.DetailedDataRequestCoroutine(c));
        }
        [Test]
        public async Task Test_Internal_Request_Return_200_And_Json_Data_Async()
        {
            var (request, expected) = Setup_Test_Internal_Request_Return_200_And_Json_Data();
            await TestSimpleAndDetailedRequestAsync(
                expected,
                request.DataRequestAsync<TestResponseData>(), request.DetailedDataRequestAsync<TestResponseData>());
        }
        private static (TestDataRequest, TestResponseData) Setup_Test_Internal_Request_Return_201_And_Xml_Data()
        {
            var provider = new TestWebRequestAdaptorProvider
            {
                FixedContentType = "application/xml",
                FixedRawResponse = "<TestResponseData><a>3</a></TestResponseData>",
                Code = HttpStatusCode.Created
            };
            IWebRequestAdaptorProvider.Current = provider;
            return (TestDataRequest.Create(), new TestResponseData
            {
                a = 3
            });
        }
        [UnityTest]
        public IEnumerator Test_Internal_Request_Return_201_And_Xml_Data()
        {
            var (request, expected) = Setup_Test_Internal_Request_Return_201_And_Xml_Data();
            yield return TestSimpleAndDetailedRequestCoroutine(expected, 
                c => request.DataRequestCoroutine(c),
                c => request.DetailedDataRequestCoroutine(c),
                HttpStatusCode.Created);
        }
        
        private static (TestDataRequest, Texture2D) Setup_Test_Internal_Texture_Request_Return_Correct_Texture()
        {
            var result = new Texture2D(20, 20);
            var provider = new TestWebRequestAdaptorProvider
            {
                Code = HttpStatusCode.OK,
                ResponseTexture = result
            };
            IWebRequestAdaptorProvider.Current = provider;
            return (TestDataRequest.Create(), result);
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Texture_Request_Return_Correct_Texture()
        {
            var (request, expected) = Setup_Test_Internal_Texture_Request_Return_Correct_Texture();
            yield return TestSimpleAndDetailedRequestCoroutine(expected, 
                c => request.TextureRequestCoroutine(false, c),
                c => request.DetailedTextureRequestCoroutine(false, c));
        }
        
        [Test]
        public async Task Test_Internal_Texture_Request_Return_Correct_Texture_Async()
        {
            var (request, expected) = Setup_Test_Internal_Texture_Request_Return_Correct_Texture();
            await TestSimpleAndDetailedRequestAsync(
                expected,
                request.TextureRequestAsync(false), request.DetailedTextureRequestAsync(false));
        }
        
        private static (TestDataRequest, AudioClip) Setup_Test_Internal_Audio_Request_Return_Correct_Audio()
        {
            var result = AudioClip.Create("test-clip", 44000, 1, 4400, false);
            var provider = new TestWebRequestAdaptorProvider
            {
                Code = HttpStatusCode.OK,
                ResponseAudioClip = result
            };
            IWebRequestAdaptorProvider.Current = provider;
            return (TestDataRequest.Create(), result);
        }
        [UnityTest]
        public IEnumerator Test_Internal_Audio_Request_Return_Correct_Audio()
        {
            var (request, expected) = Setup_Test_Internal_Audio_Request_Return_Correct_Audio();
            yield return TestSimpleAndDetailedRequestCoroutine(expected, 
                c => request.AudioRequestCoroutine(AudioType.WAV, c),
                c => request.DetailedAudioRequestCoroutine(AudioType.WAV, c));
        }
        [Test]
        public async Task Test_Internal_Audio_Request_Return_Correct_Audio_Async()
        {
            var (request, expected) = Setup_Test_Internal_Audio_Request_Return_Correct_Audio();
            await TestSimpleAndDetailedRequestAsync(
                expected,
                request.AudioRequestAsync(AudioType.WAV), request.DetailedAudioRequestAsync(AudioType.WAV));
        }
               
        private static (TestMultipartRequest, TestResponseData, List<IMultipartFormSection>) Setup_Test_Internal_Multipart_Form_Request()
        {
            var form = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("form-key-1", "form-value-1"),
                new MultipartFormDataSection("form-key-2", "form-value-2")
            };
            var provider = new TestWebRequestAdaptorProvider
            {
                Code = HttpStatusCode.Created,
                FixedContentType = "application/json",
                FixedRawResponse = @"{""a"":5}",
            };
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestMultipartRequest.Create();
            request.MultipartFormSections.AddRange(form);
            request.ContentType = ContentType.Commons.MultipartForm;
            var result = new TestResponseData
            {
                a = 5
            };
            return (request, result, form);
        }
        [UnityTest]
        public IEnumerator Test_Internal_Multipart_Form_Request()
        {
            var (request, expected, _) = Setup_Test_Internal_Multipart_Form_Request();
            yield return TestSimpleAndDetailedRequestCoroutine(expected, 
                c => request.MultipartDataRequestCoroutine(c),
                c => request.DetailedMultipartDataRequestCoroutine(c),
                HttpStatusCode.Created);
        }
        
        [Test]
        public async Task Test_Internal_Multipart_Form_Request_Async()
        {
            var (request, expected, _) = Setup_Test_Internal_Multipart_Form_Request();
            await TestSimpleAndDetailedRequestAsync(
                expected,
                request.MultipartDataRequestAsync<TestResponseData>(), 
                request.DetailedMultipartDataRequestAsync<TestResponseData>(),
                HttpStatusCode.Created);
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_On_Error_Callback()
        {
            const string result = "internal server error response";
            const string error = "connection error";
            var provider = new TestWebRequestAdaptorProvider()            
            {
                FixedContentType = "text/plain",
                FixedRawResponse = result,
                FixedError = error,
                Code = HttpStatusCode.InternalServerError
            };
            IWebRequestAdaptorProvider.Current = provider;
            var request = TestDataRequest.Create();
            yield return request.DataRequestCoroutine<string>(null, errorCallback: err =>
            {
                Assert.AreEqual(error, err.Message);
                Assert.AreEqual(result, err.ErrorBody);
                Assert.AreEqual(HttpStatusCode.InternalServerError, err.StatusCode);
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
            var msg = @"There was an missed error ""connection error"" when trying to access the resource ""http://example.com/200"". Please provide the errorCallback to catch it";
            LogAssert.Expect(LogType.Error, msg);
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Append_Correct_Multipart_Form()
        {
            var (request, _, form) = Setup_Test_Internal_Multipart_Form_Request();
            yield return request.DetailedMultipartDataRequestCoroutine<string>(response =>
            {
                var data = ((UnityWebRequest)response.WrappedRequest).uploadHandler.data;
                CollectionAssert.AreEqual(UnityWebRequest.SerializeFormSections(form, ContentType.Commons.MultipartForm.BoundaryBytes),
                    data);
            });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Append_Correct_Json_Body()
        {
            var (request, _) = Setup_Test_Internal_Request_Return_200_And_Json_Data();
            request.BodyData = new TestResponseData
            {
                a = 10
            };
            request.Method = HttpMethod.Post;
            yield return request.DetailedDataRequestCoroutine<string>(response =>
            {
                using (response)
                {
                    var data = ((UnityWebRequest)response.WrappedRequest).uploadHandler.data;
                    Assert.AreEqual(@"{""a"":10}", Encoding.UTF8.GetString(data));
                }
            });
        }
        
        [UnityTest]
        public IEnumerator Test_Internal_Request_Append_Bearer_Header()
        {
            var provider = new TestWebRequestAdaptorProvider();
            IWebRequestAdaptorProvider.Current = provider;
            SecretRepository.Save("test-key", "my-token");
            var request = TestDataRequest.Create();
            request.AuthKey = "test-key";
            string header = null;
            yield return request.DetailedDataRequestCoroutine<string>(s =>
            {
                using (s)
                {
                    header = ((UnityWebRequest)s.WrappedRequest).GetRequestHeader("Authorization");
                }
            });
            Assert.AreEqual("Bearer my-token", header);
        }

        [Serializable]
        public class TestResponseData
        {
            public int a;

            public bool Equals(TestResponseData other)
            {
                return a == other.a;
            }
            public override bool Equals(object obj)
            {
                if (obj is not TestResponseData comp)
                    return false;
                return comp.a == a;
            }
            public override int GetHashCode()
            {
                return a;
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
                Method = HttpMethod.Post;
            }
        }

        private class TestWebRequestAdaptorProvider : IWebRequestAdaptorProvider
        {
            public string FixedContentType { get; set; }
            public string FixedRawResponse { get; set; }
            public HttpStatusCode Code { get; set; } = HttpStatusCode.OK;
            public string FixedError { get; set; }
            public Texture2D ResponseTexture { get; set; }
            public AudioClip ResponseAudioClip { get; set; }
            private readonly IWebRequestAdaptorProvider _wrapped = new UnityWebRequestAdaptorProvider();
   

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
            public IWebRequestAdaptor<TBody> GetMultipartFileRequest<TBody>(string url, HttpMethod method, List<IMultipartFormSection> data, byte[] boundary)
            {
                var wrapped = _wrapped.GetMultipartFileRequest<TBody>(url, method, data, boundary) as MultipartFileUnityWebRequestAdaptor<TBody>;
                return new MultipartTestWebRequestAdaptor<TBody>(wrapped, FixedContentType, FixedRawResponse, Code, FixedError);
            }


            public IWebRequestAdaptor<TBody> GetDataRequest<TBody>(string url, HttpMethod method, string bodyData, string contentType)
            {
                var wrapped = _wrapped.GetDataRequest<TBody>(url, method, bodyData, contentType) as DataUnityWebRequestAdaptor<TBody>;
                return new DataTestWebRequestAdaptor<TBody>(wrapped, FixedContentType, FixedRawResponse, Code, FixedError);
            }


        }

        private class DataTestWebRequestAdaptor<TResponse> : BaseTestWebRequestAdaptor<
            DataUnityWebRequestAdaptor<TResponse>, TResponse>
        {
            public DataTestWebRequestAdaptor(DataUnityWebRequestAdaptor<TResponse> webRequest,
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
        private class MultipartTestWebRequestAdaptor<TResponse> : BaseTestWebRequestAdaptor<
            MultipartFileUnityWebRequestAdaptor<TResponse>, TResponse>
        {
            public MultipartTestWebRequestAdaptor(MultipartFileUnityWebRequestAdaptor<TResponse> webRequest,
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

        private abstract class BaseTestWebRequestAdaptor<TWrappedAdaptor, TResponse> : IWebRequestAdaptor<TResponse>
            where TWrappedAdaptor : UnityWebRequestAdaptor<TWrappedAdaptor, TResponse>, new()
        {
            public abstract IEnumerator RequestInstruction { get; }

            public void Dispose()
            {
                Wrapped.Dispose();
            }
            public string Url { get => Wrapped.Url;
                set => Wrapped.Url = value;
            }
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
            
            public readonly struct InternalWebResponse : IWebResponse<TResponse>
            {
                public void Dispose()
                {
                    _request?.Dispose();
                }
                public InternalWebResponse(UnityWebRequest wrappedRequest, 
                    HttpStatusCode statusCode, ContentType contentType, 
                    string rawData, TResponse data)
                {
                    _request = wrappedRequest;
                    StatusCode = statusCode;
                    ContentType = contentType;
                    RawText = rawData;
                    Data = data;
                    Headers = null;
                    RawData = null;
                }
                private readonly UnityWebRequest _request;
                public object WrappedRequest => _request;
                public HttpStatusCode StatusCode { get; }
                public ContentType ContentType { get; }
                public IEnumerable<KeyValuePair<string, string>> Headers { get; }
                public string RawText { get; }
                public byte[] RawData { get; }
                public TResponse Data { get; }
            }

            public IWebResponse<TResponse> WebResponse => new InternalWebResponse(Wrapped.WebRequest,
                StatusCode,
                IContentTypeParser.Current.ParseContentTypeFromHeader(FixedContentType),
                FixedRawResponse ,
                ResponseData
            );
            public TResponse ResponseData { get; protected set; }
        }
    }
}