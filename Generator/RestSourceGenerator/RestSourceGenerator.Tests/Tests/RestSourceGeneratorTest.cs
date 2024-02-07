using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Text;
using RestSourceGenerator.Metadata;
using RestSourceGenerator.Utilities;
using Xunit;

namespace RestSourceGenerator.Tests.Tests;

public class RestSourceGeneratorTest :
    CSharpSourceGeneratorTest<Generators.RestSourceGenerator, XUnitVerifier>
{
    protected override string DefaultTestProjectName => "SummerRest";

    [Fact]
    public void Test_Generate_Domain_Class()
    {
        var request = new Request
        {
            TypeName = "Domain",
            EndpointName = "My domain"
        };
        var builder = new StringBuilder();
        request.BuildClass(builder);
        var result =
            """
public static class MyDomain { } 
""".FormatCode();
        Assert.Equal(result, builder.FormatCode());
    }

    [Fact]
    public void Test_Generate_Domain_And_Service()
    {
        var request = new Request
        {
            TypeName = "Domain",
            EndpointName = "My domain",
            Services = new[]
            {
                new Request
                {
                    TypeName = "Service",
                    EndpointName = "1 Service"
                }
            }
        };
        var builder = new StringBuilder();
        request.BuildClass(builder);
        var result =
            """
public static class MyDomain {
    public static class _1Service { } 
}
""".FormatCode();
        Assert.Equal(result, builder.FormatCode());
    }

    [Fact]
    public void Test_Generate_Int_Field_When_Value_Greater_Than_Zero()
    {
        var timeoutResult =
            """
TimeoutSeconds = 1;
""".FormatCode();
        Assert.Equal(timeoutResult, Request.BuildIntField(1, "TimeoutSeconds"));
    }

    [Fact]
    public void Test_Generate_Empty_Field_When_Value_Less_Than_Zero()
    {
        Assert.Equal(string.Empty, Request.BuildIntField(-1, "TimeoutSeconds"));
    }

    [Fact]
    public void Test_Build_Null_Content_Type()
    {
        var (constValue, value) = Request.BuildContentType(null);
        Assert.Equal("null", constValue);
        Assert.Equal(string.Empty, value);
    }

    [Fact]
    public void Test_Build_Content_Type()
    {
        var (constValue, value) = Request.BuildContentType(new ContentType
        {
            Charset = "ISO-8859-1",
            MediaType = "application/JSON"
        });
        Assert.Equal(
            @"new ContentType(SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Application.Json, ""ISO-8859-1"", """")",
            constValue);
        Assert.Equal("ContentType = Values.ContentType;", value);
    }

    [Fact]
    public void Test_Build_Null_Url_Format()
    {
        var request = new Request
        {
            UrlFormat = null,
            UrlFormatContainers = null
        };
        var (format, keys, values, valuesArr) = request.BuildUrlFormat();
        Assert.Equal("null", format);
        Assert.Equal(string.Empty, keys);
        Assert.Equal(string.Empty, values);
        Assert.Equal("System.Array.Empty<string>()", valuesArr);
    }

    [Fact]
    public void Test_Build_Url_Format_With_Array_Of_Values()
    {
        var request = new Request
        {
            UrlFormat = "{0}/{1}",
            UrlFormatContainers = new[]
            {
                new KeyValue("key-1", "value-1"),
                new KeyValue("key-2", "value-2")
            }
        };
        var (format, keys, values, valuesArr) = request.BuildUrlFormat();
        Assert.Equal(@"""{0}/{1}""", format);
        const string expectedKeys = """
public const int Key1 = 0;public const int Key2 = 1;
""";
        Assert.Equal(expectedKeys, keys);
        const string expectedValues = """
public const string Key1 = "value-1";public const string Key2 = "value-2";
""";
        Assert.Equal(expectedValues, values);
        const string expectedRefValues = """
new string[] {Values.UrlFormat.Key1,Values.UrlFormat.Key2}
""";
        Assert.Equal(expectedRefValues, valuesArr);
    }

    [Fact]
    public void Test_Build_Null_Key_Values_RefValues()
    {
        var (keys, values, refValues) = Request.BuildKeysValuesRefValues(null, null, null);
        Assert.Equal(string.Empty, keys);
        Assert.Equal(string.Empty, values);
        Assert.Equal(string.Empty, refValues);
    }

    [Fact]
    public void Test_Build_Key_Values_RefValues()
    {
        var headers = new KeyValue[]
        {
            new("header-1", "header-1-value"),
            new("header-2", "header-2-value"),
            new("header-3", null),
        };
        var (keys, values, refValues) = Request.BuildKeysValuesRefValues(headers, "Headers",
            kv => $@"Headers.TryAdd({kv.Key}, {kv.Value})");
        const string expectedKeys = """
public const string Header1 = "header-1";public const string Header2 = "header-2";public const string Header3 = "header-3";
""";
        Assert.Equal(expectedKeys, keys);
        const string expectedValues = """
public const string Header1 = "header-1-value";public const string Header2 = "header-2-value";
""";
        Assert.Equal(expectedValues, values);
        const string expectedRefValues = """
Headers.TryAdd(Keys.Headers.Header1, Values.Headers.Header1);Headers.TryAdd(Keys.Headers.Header2, Values.Headers.Header2);
""";
        Assert.Equal(expectedRefValues, refValues);
    }
    
    [Fact]
    public void Test_Build_Param_Multiple_Values_RefValues()
    {
        var headers = new KeyValue[]
        {
            new("param-1", "param-1-value-1"),
            new("param-1", "param-1-value-2"),
            new("param-2", "param-2-value"),
        };
        var (keys, values, refValues) = Request.BuildKeysValuesRefValues(headers, "Params",
            kv => $@"Params.AddParamToList({kv.Key}, {kv.Value})");
        const string expectedKeys = """
public const string Param1 = "param-1";public const string Param2 = "param-2";
""";
        Assert.Equal(expectedKeys, keys);
        const string expectedValues = """
public const string Param2 = "param-2-value";public const string[] Param1 = new string[] {"param-1-value-1","param-1-value-2"};
""";
        Assert.Equal(expectedValues, values);
        const string expectedRefValues = """
Params.AddParamToList(Keys.Params.Param1, Values.Params.Param1);Params.AddParamToList(Keys.Params.Param2, Values.Params.Param2);
""";
        Assert.Equal(expectedRefValues, refValues);
    }


    [Fact]
    public void Test_Build_Null_Auth()
    {
        var (authProp, authKey) = Request.BuildAuth(null);
        Assert.Equal("null", authProp);
        Assert.Equal(string.Empty, authKey);
    }

    [Fact]
    public void Test_Build_Auth()
    {
        var (authProp, authKey) = Request.BuildAuth(new AuthContainer
        {
            AuthKey = "my-json-token",
            AppenderType = "BearerTokenAuthAppender",
            AuthDataType = "string"
        });
        const string authExpected = """
AuthKey = SummerRest.Runtime.Authenticate.AuthKeys.MyJsonToken;
""";
        Assert.Equal(authExpected, authKey);
        const string authPropExpected = """
IRequestModifier<AuthRequestModifier<BearerTokenAuthAppender, string>>.GetSingleton()
""";
        Assert.Equal(authPropExpected, authProp);
    }

    [Fact]
    public void Test_Build_Null_Data_Body()
    {
        var (serializedVal, body) = Request.BuildDataBody(null, DataFormat.Json);
        Assert.Equal("null", serializedVal);
        const string expectedBody = "BodyFormat = DataFormat.Json;";
        Assert.Equal(expectedBody, body);
    }

    [Fact]
    public void Test_Build_Data_Body()
    {
        var (serializedVal, body) = Request.BuildDataBody(@"I am a ""cat"" request", DataFormat.Json);
        Assert.Equal(@"@""I am a """"cat"""" request""", serializedVal);
        const string expectedBody =
            """
BodyFormat = DataFormat.Json;
InitializedSerializedBody = Values.SerializedBody;
""";
        Assert.Equal(expectedBody, body);
    }

    [Fact]
    public void Test_Build_Auth_Keys()
    {
        var conf = new Configuration
        {
            AuthKeys = new[]
            {
                "access Key",
                "1 json_key"
            }
        };
        var authKeyClass = conf.BuildAuthClass();
        const string expected =
            """
public const string AccessKey = "access Key";public const string _1Json_key = "1 json_key";
""";
        Assert.Equal(expected, authKeyClass);
    }

    [Fact]
    public void Test_Build_Data_Request()
    {
        var request = new Request
        {
            TypeName = "Request",
            EndpointName = "request 1",
            UrlFormat = "my-url.com/{0}/{1}",
            UrlFormatContainers = new[]
            {
                new KeyValue("product-id", "1"),
                new KeyValue("image-order", "2"),
            },
            Url = "my-url.com/1/2",
            UrlWithParams = "my-url.com/1/2",
            Headers = new[]
            {
                new KeyValue("Header-1", "Value-1"),
                new KeyValue("Header-2", "Value-2"),
            },
            IsMultipart = false,
            TimeoutSeconds = 3,
            DataFormat = DataFormat.Json,
            SerializedBody = "my data body",
            ContentType = new ContentType("utf-8", "application/json", string.Empty)
        };
        const string expected =
            """
public sealed class Request1 : SummerRest.Runtime.Requests.BaseDataRequest<Request1>
{
   public static class Keys
   {
       public static class UrlFormat
       {
            public const int ProductId = 0;
            public const int ImageOrder = 1;
       }
       public static class Headers
       {
           public const string Header1 = "Header-1";
           public const string Header2 = "Header-2";
       }
       public static class Params
       {
       }
       public static class MultipartFormSections
       {
       }
   }
    public static class Values {
        public const string Url = "my-url.com/1/2";
        public const string UrlWithParams = "my-url.com/1/2";
        public const string UrlFormatPattern = "my-url.com/{0}/{1}";
        public const string SerializedBody = @"my data body";
        public static readonly ContentType? ContentType = new ContentType(SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Application.Json, SummerRest.Runtime.RequestComponents.ContentType.Encodings.Utf8, "");
        public static class UrlFormat {
            public const string ProductId = "1";
            public const string ImageOrder = "2";
        }
        public static class Headers {
           public const string Header1 = "Value-1";
           public const string Header2 = "Value-2";
        }
        public static class Params {
        }
        public static class MultipartFormSections {
        }
    }
   public Request1(): base(Values.Url, Values.UrlWithParams, Values.UrlFormatPattern, new string[]{Values.UrlFormat.ProductId, Values.UrlFormat.ImageOrder}, null)
   {
       Method = HttpMethod.Get;
       TimeoutSeconds = 3;
       ContentType = Values.ContentType;
       Headers.TryAdd(Keys.Headers.Header1, Values.Headers.Header1);
       Headers.TryAdd(Keys.Headers.Header2, Values.Headers.Header2);
       BodyFormat = DataFormat.Json;
       InitializedSerializedBody = Values.SerializedBody;
       Init();
   }
}
""";
        var builder = new StringBuilder();
        request.BuildClass(builder);
        Assert.Equal(expected.FormatCode(), builder.FormatCode());
    }

        [Fact]
    public void Test_Build_Multipart_Request()
    {
        var request = new Request
        {
            TypeName = "Request",
            EndpointName = "Multipart request",
            Url = "my-url.com",
            RequestParams = new []
            {
              new KeyValue("param1", "value-1"),
              new KeyValue("param_2", "value-2"),
            },
            UrlWithParams = "my-url.com?param1=value1&param_2=value2",
            AuthContainer = new AuthContainer
            {
                AuthKey = "my-key",
                AppenderType = "BearerTokenAppender",
                AuthDataType = "string"
            },
            ContentType = new ContentType("utf-8", "multipart/form-data", "abcd1234"),
            IsMultipart = true,
            SerializedForm = new []
            {
                new KeyValue("form-key-1", "form-value-1"),
                new KeyValue("form-key-2", "form-value-2"),
                new KeyValue("form-file", null),
            }
        };
        const string expected =
            """
public sealed class MultipartRequest : SummerRest.Runtime.Requests.BaseMultipartRequest<MultipartRequest>
{
   public static class Keys
   {
       public static class UrlFormat
       {
       }
       public static class Headers
       {
       }
       public static class Params
       {
           public const string Param_2 = "param_2";
           public const string Param1 = "param1";
       }
       public static class MultipartFormSections
       {
           public const string FormFile = "form-file";
           public const string FormKey1 = "form-key-1";
           public const string FormKey2 = "form-key-2";
       }
   }
    public static class Values {
        public const string Url = "my-url.com";
        public const string UrlWithParams = "my-url.com?param1=value1&param_2=value2";
        public const string UrlFormatPattern = null;
        public const string SerializedBody = null;
        public static readonly ContentType? ContentType = new ContentType(SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Multipart.FormData, SummerRest.Runtime.RequestComponents.ContentType.Encodings.Utf8, "abcd1234");
        public static class UrlFormat {
        }
        public static class Headers {
        }
        public static class Params {
           public const string Param_2 = "value-2";
           public const string Param1 = "value-1";
        }
        public static class MultipartFormSections {
           public const string FormKey1 = "form-value-1";
           public const string FormKey2 = "form-value-2";
        }
    }
   public MultipartRequest(): base(Values.Url, Values.UrlWithParams, Values.UrlFormatPattern, System.Array.Empty<string>(), IRequestModifier<AuthRequestModifier<BearerTokenAppender, string>>.GetSingleton())
   {
       Method = HttpMethod.Get;
       ContentType = Values.ContentType;
       Params.AddParamToList(Keys.Params.Param_2, Values.Params.Param_2);
       Params.AddParamToList(Keys.Params.Param1, Values.Params.Param1);
       AuthKey = SummerRest.Runtime.Authenticate.AuthKeys.MyKey;
       MultipartFormSections.Add(new MultipartFormDataSection(Keys.MultipartFormSections.FormKey1, Values.MultipartFormSections.FormKey1));
       MultipartFormSections.Add(new MultipartFormDataSection(Keys.MultipartFormSections.FormKey2, Values.MultipartFormSections.FormKey2));
       Init();
   }
}
""";
        var builder = new StringBuilder();
        request.BuildClass(builder);
        Assert.Equal(expected.FormatCode(), builder.FormatCode());
    }

    
    [Fact]
    public async Task Test_Generate_Data_Request_From_Xml()
    {
        var json = """
                   <SummerRestConfiguration Assembly="SummerRest">
                       <AuthKeys>
                           <string>my-token-to-master-service</string>
                       </AuthKeys>
                       <Domains>
                           <Domain TypeName="Domain" EndpointName="Domain1">
                               <Services>
                                   <Service TypeName="Service" EndpointName="MyService">
                                       <Requests>
                                           <Request TypeName="Request" EndpointName="Request 1" Url="http://domain1.com/service1/get/1" UrlFormat="http://domain1.com/service1/get/{0}" UrlWithParams="http://domain1.com/service1/get/1?param1=param1-value" Method="Get" TimeoutSeconds="0" DataFormat="Json" SerializedBody="I need to call the &quot;cat&quot; request" IsMultipart="false">
                                               <ContentType Charset="UTF-8" MediaType="text/plain" />
                                               <Headers>
                                                   <KeyValue Key="header1" Value="value1" />
                                                   <KeyValue Key="1" Value="value2" />
                                               </Headers>
                                               <RequestParams>
                                                   <KeyValue Key="param1" Value="param1-value" />
                                               </RequestParams>
                                               <UrlFormatContainers>
                                                   <KeyValue Key="format-key-1" Value="format-value-1" />
                                               </UrlFormatContainers>
                                               <AuthContainer AuthKey="my-token-to-master-service" AppenderType="SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender" AuthDataType="System.String" />
                                           </Request>
                                       </Requests>
                                   </Service>
                               </Services>
                           </Domain>
                       </Domains>
                   </SummerRestConfiguration>
                   """;

        var expected = """
                       using SummerRest.Runtime.RequestComponents;
                       using SummerRest.Runtime.Parsers;
                       using UnityEngine.Networking;
                       namespace SummerRest.Runtime.Authenticate
                       {
                           public static class AuthKeys
                           {
                               public const string MyTokenToMasterService = "my-token-to-master-service";
                           }
                       }
                       namespace SummerRest.Runtime.Requests
                       {
                           public static class Domain1
                           {
                               public static class MyService
                               {
                                   public sealed class Request1 : SummerRest.Runtime.Requests.BaseDataRequest<Request1>
                                   {
                                       public static class Keys
                                       {
                                           public static class UrlFormat
                                           {
                                                public const int FormatKey1 = 0;
                                           }
                                           public static class Headers
                                           {
                                               public const string _1 = "1";
                                               public const string Header1 = "header1";
                                           }
                                           public static class Params
                                           {
                                               public const string Param1 = "param1";
                                           }
                                           public static class MultipartFormSections
                                           {
                                           }
                                       }
                                        public static class Values {
                                            public const string Url = "http://domain1.com/service1/get/1";
                                            public const string UrlWithParams = "http://domain1.com/service1/get/1?param1=param1-value";
                                            public const string UrlFormatPattern = "http://domain1.com/service1/get/{0}";
                                            public const string SerializedBody = @"I need to call the ""cat"" request";
                                            public static readonly ContentType? ContentType = new ContentType(SummerRest.Runtime.RequestComponents.ContentType.MediaTypeNames.Text.Plain, SummerRest.Runtime.RequestComponents.ContentType.Encodings.Utf8, "");
                                            public static class UrlFormat {
                                                public const string FormatKey1 = "format-value-1";
                                            }
                                            public static class Headers {
                                                public const string _1 = "value2";
                                                public const string Header1 = "value1";
                                            }
                                            public static class Params {
                                                public const string Param1 = "param1-value";
                                            }
                                            public static class MultipartFormSections {
                                                
                                            }
                                        }
                                       public Request1(): base(Values.Url, Values.UrlWithParams, Values.UrlFormatPattern, new string[]{Values.UrlFormat.FormatKey1}, IRequestModifier<AuthRequestModifier<SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender, System.String>>.GetSingleton())
                                       {
                                           Method = HttpMethod.Get;
                                           TimeoutSeconds = 0;
                                           ContentType = Values.ContentType;
                                           Headers.TryAdd(Keys.Headers._1, Values.Headers._1);
                                           Headers.TryAdd(Keys.Headers.Header1, Values.Headers.Header1);
                                           Params.AddParamToList(Keys.Params.Param1, Values.Params.Param1);
                                           AuthKey = SummerRest.Runtime.Authenticate.AuthKeys.MyTokenToMasterService;
                                           BodyFormat = DataFormat.Json;
                                           InitializedSerializedBody = Values.SerializedBody;
                                           Init();
                                       }
                                   }
                               }
                           }
                       }
                       """;

        var test = new RestSourceGeneratorTest
        {
            TestState =
            {
                AdditionalFiles = { ("summer-rest-generated.RestSourceGenerator.additionalfile", json) },
                ExpectedDiagnostics =
                {
                    new DiagnosticResult(new DiagnosticDescriptor("RestSourceGenerator", "Start generating",
                        "Start generating source", "Debug", DiagnosticSeverity.Info, true)),
                    new DiagnosticResult(new DiagnosticDescriptor("RestSourceGenerator", "Finish generating",
                        "Finish generating source", "Debug", DiagnosticSeverity.Info, true))
                },
                GeneratedSources =
                    { (typeof(Generators.RestSourceGenerator), "SummerRestRequests.g.cs", expected.FormatCode()) }
            },
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        await test.RunAsync();
    }
}