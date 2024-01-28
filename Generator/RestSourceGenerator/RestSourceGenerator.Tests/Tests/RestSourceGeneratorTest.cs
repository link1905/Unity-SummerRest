using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using RestSourceGenerator.Utilities;
using Xunit;

namespace RestSourceGenerator.Tests.Tests;

public class RestSourceGeneratorTest :
    CSharpSourceGeneratorTest<Generators.RestSourceGenerator, XUnitVerifier>
{
    protected override string DefaultTestProjectName => "SummerRest";

    private Task RunTest(string jsonContent, string expected)
    {
      var test = new RestSourceGeneratorTest
      {
        TestState =
        {
          AdditionalFiles = { ("summer-rest-generated.RestSourceGenerator.additionalfile", jsonContent) },
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
      return test.RunAsync();
    }
    [Fact]
    public async Task Test_Generate_Request_From_Json()
    {
        var json = """
                   {
                      "Assembly": "SummerRest",
                      "Domains": [
                     {
                       "TypeName": "Domain",
                       "Services": [
                         {
                           "TypeName": "Service",
                           "Services": [],
                           "Requests": [
                             {
                               "UrlWithParams": "example2.com/service1/asdasdas?123123=aaaaaa",
                               "Method": 0,
                               "RequestParams": [
                                 {
                                   "Key": "123123",
                                   "Value": "aaaaaa"
                                 }
                               ],
                               "RequestBody": {
                                 "SerializedData": "I need to call the \"cat\" request"
                               },
                               "SerializedBody": "I need to call the \"cat\" request",
                               "TypeName": "Request",
                               "EndpointName": "Request 1",
                               "Url": "example2.com/service1/asdasdas",
                               "Path": "asdasdas",
                               "DataFormat": 0,
                               "AuthContainer": {
                                 "AuthKey": "my token",
                                 "AppenderType": "SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender",
                                 "AuthDataType": "System.String"
                               },
                               "IsMultipart": false,
                               "Headers": [
                                 {
                                   "Key": "header2",
                                   "Value": "value2"
                                 },
                                 {
                                   "Key": "header1",
                                   "Value": "value1"
                                 }
                               ],
                               "ContentType": {
                                 "Charset": "UTF-8",
                                 "MediaType": "application/json",
                                 "Boundary": "",
                                 "FormedContentType": "application/json; charset=UTF-8"
                               },
                               "TimeoutSeconds": 0
                             }
                           ],
                           "EndpointName": "My service"
                         }
                       ],
                       "Requests": [],
                       "EndpointName": "Domain 1"
                     }
                   ]
                   }
                   """;

        var expected = """

                          using SummerRest.Runtime.RequestComponents;
                          using SummerRest.Runtime.Parsers;
                          namespace SummerRest.Runtime.Requests {
                           public static class Domain1 {
                               public static class MyService {
                                   public class Request1 : SummerRest.Runtime.Requests.BaseDataRequest<Request1> {
                                        public Request1() : base("example2.com/service1/asdasdas", "example2.com/service1/asdasdas?123123=aaaaaa", IRequestModifier<AuthRequestModifier<SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender, System.String>>.GetSingleton())
                                        {
                                           Method = HttpMethod.Get;
                                           TimeoutSeconds = 0;
                                           ContentType = new ContentType("application/json", "UTF-8", "");
                                           Headers.Add("header2", "value2");
                                           Headers.Add("header1", "value1");
                                           Params.AddParamToList("123123", "aaaaaa");
                                           AuthKey = "my token";
                                           BodyFormat = DataFormat.Json;
                                           InitializedSerializedBody = @"I need to call the ""cat"" request";
                                           Init();
                                       }
                                   }
                               }
                           }
                       }

                       """;

        await RunTest(json, expected);
    }
    
    
    [Fact]
    public async Task Test_Generate_Complex_Api_Structure_From_Json()
    {
        var json = """
                   {
                     "Domains": [
                       {
                         "TypeName": "Domain",
                         "EndpointName": "MyJsonDomain",
                         "Requests": [],
                         "Services": [
                           {
                             "TypeName": "Service",
                             "EndpointName": "Data service",
                             "Requests": [
                               {
                                 "UrlWithParams": "http://my-domain.com/data?param-1=param-value-1&param-2=param-value-2",
                                 "Method": 0,
                                 "RequestParams": [
                                   {
                                     "Key": "param-1",
                                     "Value": "param-value-1"
                                   },
                                   {
                                     "Key": "param-2",
                                     "Value": "param-value-2"
                                   }
                                 ],
                                 "RequestBody": {},
                                 "IsMultipart": true,
                                 "SerializedForm": [
                                   {
                                     "Key": "form-key-1",
                                     "Value": "form-value-1"
                                   },
                                   {
                                     "Key": "form-key-2",
                                     "Value": "form-value-2"
                                   }
                                 ],
                                 "TypeName": "Request",
                                 "EndpointName": "GetRequest",
                                 "Url": "http://my-domain.com/data",
                                 "Path": "",
                                 "DataFormat": 0,
                                 "AuthContainer": null,
                                 "Headers": [
                                   {
                                     "Key": "header-1",
                                     "Value": "header-value-1"
                                   },
                                   {
                                     "Key": "header-2",
                                     "Value": "header-value-2"
                                   }
                                 ],
                                 "ContentType": null,
                                 "TimeoutSeconds": null,
                                 "RedirectsLimit": null,
                                 "name": "22ed4ac379091f9469987cc21a0f859e",
                                 "hideFlags": 0
                               },
                               {
                                 "UrlWithParams": "http://my-domain.com/data",
                                 "Method": 1,
                                 "RequestParams": [],
                                 "RequestBody": {},
                                 "SerializedBody": "i am a big cat",
                                 "TypeName": "Request",
                                 "EndpointName": "PostRequest",
                                 "Url": "http://my-domain.com/data",
                                 "Path": "",
                                 "DataFormat": 0,
                                 "AuthContainer": {
                                   "AuthKey": "my-json-key",
                                   "AppenderType": "SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender",
                                   "AuthDataType": "System.String"
                                 },
                                 "Headers": [
                                   {
                                     "Key": "header-1",
                                     "Value": "header-value-1"
                                   },
                                   {
                                     "Key": "header-2",
                                     "Value": "header-value-2"
                                   }
                                 ],
                                 "ContentType": null,
                                 "TimeoutSeconds": null,
                                 "RedirectsLimit": null,
                                 "name": "14dceaa44b9f26e46a0f1c999e67dc0b",
                                 "hideFlags": 0
                               }
                             ],
                             "Services": [
                               {
                                 "TypeName": "Service",
                                 "EndpointName": "AccountService",
                                 "Requests": [
                                   {
                                     "UrlWithParams": "http://my-domain.com/data/account",
                                     "Method": 2,
                                     "RequestParams": [],
                                     "RequestBody": {},
                                     "SerializedBody": "i need a cat now",
                                     "TypeName": "Request",
                                     "EndpointName": "PutRequest",
                                     "Url": "http://my-domain.com/data/account",
                                     "Path": "",
                                     "DataFormat": 0,
                                     "AuthContainer": null,
                                     "Headers": [
                                       {
                                         "Key": "header-3",
                                         "Value": "header-3-value"
                                       },
                                       {
                                         "Key": "header-1",
                                         "Value": "header-value-1"
                                       },
                                       {
                                         "Key": "header-2",
                                         "Value": "header-value-2"
                                       }
                                     ],
                                     "ContentType": {
                                       "Charset": "UTF-8",
                                       "MediaType": "application/json",
                                       "Boundary": "",
                                       "FormedContentType": "application/json; charset=UTF-8"
                                     },
                                     "TimeoutSeconds": 3,
                                     "RedirectsLimit": null,
                                     "name": "2c74bfad2c736224a9d87d8f619e2ae9",
                                     "hideFlags": 0
                                   }
                                 ],
                                 "Services": []
                               }
                             ]
                           },
                           {
                             "TypeName": "Service",
                             "EndpointName": "Image service",
                             "Requests": [
                               {
                                 "UrlWithParams": "http://my-domain.com/image",
                                 "Method": 0,
                                 "RequestParams": [],
                                 "RequestBody": {},
                                 "SerializedBody": "",
                                 "TypeName": "Request",
                                 "EndpointName": "GetImage",
                                 "Url": "http://my-domain.com/image",
                                 "Path": "",
                                 "DataFormat": 0,
                                 "AuthContainer": null,
                                 "Headers": null,
                                 "ContentType": {
                                   "Charset": "UTF-8",
                                   "MediaType": "image/png",
                                   "Boundary": "",
                                   "FormedContentType": "image/png; charset=UTF-8"
                                 },
                                 "TimeoutSeconds": 3,
                                 "RedirectsLimit": 3,
                                 "name": "8a02c3390427d2747841b245678483ea",
                                 "hideFlags": 0
                               }
                             ],
                             "Services": []
                           }
                         ]
                       },
                       {
                         "TypeName": "Domain",
                         "EndpointName": "MySoapDomain",
                         "Requests": [
                           {
                             "UrlWithParams": "http://localhost:8080/?soap-param-1=soap-value-1&soap-param-1=soap-value-2",
                             "Method": 0,
                             "RequestParams": [
                               {
                                 "Key": "soap-param-1",
                                 "Value": "soap-value-1"
                               },
                               {
                                 "Key": "soap-param-1",
                                 "Value": "soap-value-2"
                               }
                             ],
                             "RequestBody": {},
                             "SerializedBody": "",
                             "TypeName": "Request",
                             "EndpointName": "GetRequest",
                             "Url": "http://localhost:8080/",
                             "Path": "",
                             "DataFormat": 2,
                             "AuthContainer": {
                               "AuthKey": "my-soap-key",
                               "AppenderType": "SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender",
                               "AuthDataType": "System.String"
                             },
                             "Headers": null,
                             "ContentType": {
                               "Charset": "UTF-8",
                               "MediaType": "application/soap+xml",
                               "Boundary": "",
                               "FormedContentType": "application/soap+xml; charset=UTF-8"
                             },
                             "TimeoutSeconds": null,
                             "RedirectsLimit": null,
                             "name": "173fb87c9d987e44fabb651abe2fb057",
                             "hideFlags": 0
                           }
                         ],
                         "Services": []
                       }
                     ],
                     "Assembly": "SummerRest",
                     "name": "SummerRestConfiguration",
                     "hideFlags": 0
                   }
                   """;

        var expected = """
                          using SummerRest.Runtime.RequestComponents;
                          using SummerRest.Runtime.Parsers;
                          namespace SummerRest.Runtime.Requests {
                           public static class MyJsonDomain {
                               public static class DataService {
                                   public class GetRequest : SummerRest.Runtime.Requests.BaseMultipartRequest<GetRequest> {

                                       public GetRequest() : base("http://my-domain.com/data", "http://my-domain.com/data?param-1=param-value-1&param-2=param-value-2", null)
                                       {
                                           Method = HttpMethod.Get;
                                           Headers.Add("header-1", "header-value-1");
                                           Headers.Add("header-2", "header-value-2");
                                           Params.AddParamToList("param-1", "param-value-1");
                                           Params.AddParamToList("param-2", "param-value-2");
                                           MultipartFormSections.Add(new MultipartFormDataSection("form-key-1", "form-value-1"));
                                           MultipartFormSections.Add(new MultipartFormDataSection("form-key-2", "form-value-2"));
                                           Init();
                                       }
                                   }
                                   public class PostRequest : SummerRest.Runtime.Requests.BaseDataRequest<PostRequest> {
                                       public PostRequest() : base("http://my-domain.com/data", "http://my-domain.com/data", IRequestModifier<AuthRequestModifier<SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender, System.String>>.GetSingleton())
                                       {
                                           Method = HttpMethod.Post;
                                           Headers.Add("header-1", "header-value-1");
                                           Headers.Add("header-2", "header-value-2");
                                           AuthKey = "my-json-key";
                                           BodyFormat = DataFormat.Json;
                                           InitializedSerializedBody = @"i am a big cat";
                                           Init();
                                       }
                                   }
                                   public static class AccountService {
                                      public class PutRequest : SummerRest.Runtime.Requests.BaseDataRequest<PutRequest> {

                                          public PutRequest() : base("http://my-domain.com/data/account", "http://my-domain.com/data/account", null) 
                                          {
                                              Method = HttpMethod.Put;
                                              TimeoutSeconds = 3;
                                              ContentType = new ContentType("application/json", "UTF-8", "");
                                              Headers.Add("header-3", "header-3-value");
                                              Headers.Add("header-1", "header-value-1");
                                              Headers.Add("header-2", "header-value-2");
                                              BodyFormat = DataFormat.Json;
                                              InitializedSerializedBody = @"i need a cat now";
                                              Init();
                                          }
                                      }
                                   }
                               }
                               public static class ImageService {
                                  public class GetImage : SummerRest.Runtime.Requests.BaseDataRequest<GetImage> {
                                     
                                     public GetImage() : base("http://my-domain.com/image", "http://my-domain.com/image", null)
                                     {
                                         Method = HttpMethod.Get;
                                         TimeoutSeconds = 3; 
                                         RedirectsLimit = 3; 
                                         ContentType = new ContentType("image/png", "UTF-8", "");
                                         BodyFormat = DataFormat.Json;
                                         Init();
                                     }
                                  }
                               }
                           }
                           public static class MySoapDomain {
                              public class GetRequest : SummerRest.Runtime.Requests.BaseDataRequest<GetRequest> {
                                  public GetRequest() : base("http://localhost:8080/", "http://localhost:8080/?soap-param-1=soap-value-1&soap-param-1=soap-value-2", IRequestModifier<AuthRequestModifier<SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender, System.String>>.GetSingleton())
                                  {
                                       Method = HttpMethod.Get;
                                       ContentType = new ContentType("application/soap+xml", "UTF-8", "");
                                       Params.AddParamToList("soap-param-1", "soap-value-1");
                                       Params.AddParamToList("soap-param-1", "soap-value-2");
                                       AuthKey = "my-soap-key";
                                       BodyFormat = DataFormat.Xml;
                                       Init();
                                  }
                              }
                           }
                       }

                       """;

        await RunTest(json, expected);
    }

}