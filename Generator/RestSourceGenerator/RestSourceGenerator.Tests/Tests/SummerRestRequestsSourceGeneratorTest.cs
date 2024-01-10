using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using RestSourceGenerator.Generators;
using RestSourceGenerator.Utilities;
using Xunit;

namespace RestSourceGenerator.Tests.Tests;

public class SummerRestRequestsSourceGeneratorTest :
    CSharpSourceGeneratorTest<SummerRestRequestsSourceGenerator, XUnitVerifier>
{
    protected override string DefaultTestProjectName => "SummerRest";

    [Fact]
    public async Task Test_Generate_Request_From_Json()
    {
        var json = """
                   {
                      "Assembly": "SummerRest",
                      "Domains": [
                     {
                       "ActiveVersion": "example2.com",
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
                                 "AppenderType": "SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender"
                               },
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
                               "TimeoutSeconds": 0,
                               "name": "cfce0808c22598840beaffe37a8896db",
                               "hideFlags": 0
                             }
                           ],
                           "EndpointName": "My service",
                           "Url": "example2.com/service1",
                           "Path": "service1",
                           "DataFormat": 0,
                           "AuthContainer": {
                             "AuthKey": "my token",
                             "AppenderType": "SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender"
                           },
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
                           "TimeoutSeconds": 0,
                           "name": "c46eab657db607443a354f3f32779a09",
                           "hideFlags": 0
                         }
                       ],
                       "Requests": [],
                       "EndpointName": "Domain 1",
                       "Url": "example2.com",
                       "Path": "",
                       "DataFormat": 0,
                       "AuthContainer": {
                         "AuthKey": "my token",
                         "AppenderType": "SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender"
                       },
                       "Headers": [
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
                       "TimeoutSeconds": 0,
                       "RedirectsLimit": 0,
                       "name": "1458e1cf6d8a7514cafe29bea538da78",
                       "hideFlags": 0
                     }
                   ]
                   }
                   """;

        var expected = """

                       namespace SummerRest.Requests {
                           public class Domain1 {
                               public class MyService {
                                   public class Request1 : BaseAuthRequest<Request1, SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender> {
                                       public Request1() : base("example2.com/service1/asdasdas", "example2.com/service1/asdasdas?123123=aaaaaa")
                                       {
                                           Method = HttpMethod.Get;
                                           TimeoutSeconds = 0;
                                           ContentType = new ContentType("application/json", "UTF-8", "");
                                           Headers.Add("header2", "value2");
                                           Headers.Add("header1", "value1");
                                           Params.AddParam("123123", "aaaaaa");
                                           AuthKey = "my token";
                                           Init();
                                       }
                                   }
                                   public class Request1<TRequestBody> : Request1, IWebRequest<TRequestBody> {
                                       public TRequestBody BodyData { get; set; }
                                       public DataFormat BodyFormat { get; set; }
                                       public override string SerializedBody => BodyData is null ? null : IDataSerializer.Current.Serialize(BodyData, BodyFormat);
                                       public Request1() : base("example2.com/service1/asdasdas", "example2.com/service1/asdasdas?123123=aaaaaa")
                                       {
                                           BodyFormat = DataFormat.Json;
                                           BodyData = DefaultDataSerializer.StaticDeserialize<TRequestBody>(@"I need to call the ""cat"" request", DataFormat.Json);
                                        }
                                   }
                               }
                           }
                       }

                       """;

        var test = new SummerRestRequestsSourceGeneratorTest
        {
            TestState =
            {
                AdditionalFiles = { ("summer-rest-generated.SummerRestRequestsGenerator.additionalfile", json) },
                GeneratedSources =
                    { (typeof(SummerRestRequestsSourceGenerator), "SummerRestRequests.g.cs", expected.FormatCode()) }
            },
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        await test.RunAsync();
    }
}