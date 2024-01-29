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
    public async Task Test_Generate_Request_From_Xml()
    {
        var json = """
                   <SummerRestConfiguration Assembly="SummerRest">
                       <Domains>
                           <Domain TypeName="Domain" EndpointName="Domain1">
                               <Services>
                                   <Service TypeName="Service" EndpointName="MyService">
                                       <Requests>
                                           <Request TypeName="Request" EndpointName="Request 1" Url="http://domain1.com/service1/get" UrlWithParams="http://domain1.com/service1/get?param1=param1-value" Method="Get" TimeoutSeconds="0" DataFormat="Json" SerializedBody="I need to call the &quot;cat&quot; request" IsMultipart="false">
                                               <ContentType Charset="UTF-8" MediaType="text/plain" />
                                               <Headers>
                                                   <KeyValue Key="header1" Value="value1" />
                                                   <KeyValue Key="1" Value="value2" />
                                               </Headers>
                                               <RequestParams>
                                                   <KeyValue Key="param1" Value="param1-value" />
                                               </RequestParams>
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
                          namespace SummerRest.Runtime.Requests {
                           public static class Domain1 {
                               public static class MyService {
                                   public class Request1 : SummerRest.Runtime.Requests.BaseDataRequest<Request1> {
                                        public Request1() : base("http://domain1.com/service1/get", "http://domain1.com/service1/get?param1=param1-value", IRequestModifier<AuthRequestModifier<SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender, System.String>>.GetSingleton())
                                        {
                                           Method = HttpMethod.Get;
                                           TimeoutSeconds = 0;
                                           ContentType = new ContentType("text/plain", "UTF-8", "");
                                           Headers.Add("header1", "value1");
                                           Headers.Add("1", "value2");
                                           Params.AddParamToList("param1", "param1-value");
                                           AuthKey = "my-token-to-master-service";
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
    public async Task Test_Generate_Complex_Api_Structure_From_Xml()
    {
        var json = """
                   <SummerRestConfiguration Assembly="SummerRest">
                       <Domains>
                           <Domain TypeName="Domain" EndpointName="JsonPlaceHolder">
                               <Services>
                                   <Service TypeName="Service" EndpointName="Comments">
                                       <Requests>
                                           <Request TypeName="Request" EndpointName="GetPost1Comments" Url="https://jsonplaceholder.typicode.com/comments" UrlWithParams="https://jsonplaceholder.typicode.com/comments?postId=1" Method="Get" DataFormat="Json" SerializedBody="" IsMultipart="false">
                                               <RequestParams>
                                                   <KeyValue Key="postId" Value="1" />
                                               </RequestParams>
                                               <AuthContainer AuthKey="my-token-to-master-service" AppenderType="SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender" AuthDataType="System.String" />
                                           </Request>
                                       </Requests>
                                   </Service>
                                   <Service TypeName="Service" EndpointName="Posts">
                                       <Requests>
                                           <Request TypeName="Request" EndpointName="Get1" Url="https://jsonplaceholder.typicode.com/posts/1" UrlWithParams="https://jsonplaceholder.typicode.com/posts/1" Method="Get" DataFormat="PlainText" SerializedBody="" IsMultipart="false">
                                               <AuthContainer AuthKey="my-token-to-master-service" AppenderType="SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender" AuthDataType="System.String" />
                                           </Request>
                                           <Request TypeName="Request" EndpointName="Post" Url="https://jsonplaceholder.typicode.com/posts" UrlWithParams="https://jsonplaceholder.typicode.com/posts" Method="Post" DataFormat="Json" SerializedBody="" IsMultipart="false">
                                               <AuthContainer AuthKey="my-token-to-master-service" AppenderType="SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender" AuthDataType="System.String" />
                                           </Request>
                                       </Requests>
                                   </Service>
                               </Services>
                           </Domain>
                           <Domain TypeName="Domain" EndpointName="NinjaApiWithAuth">
                               <Requests>
                                   <Request TypeName="Request" EndpointName="GetRandomWord" Url="https://api.api-ninjas.com/v1/randomword" UrlWithParams="https://api.api-ninjas.com/v1/randomword" Method="Get" DataFormat="Json" SerializedBody="" IsMultipart="false">
                                       <AuthContainer AuthKey="ninja-api-token" AppenderType="SummerRestSample.NinjaApiAuthAppender" AuthDataType="System.String" />
                                   </Request>
                               </Requests>
                           </Domain>
                           <Domain TypeName="Domain" EndpointName="Multipart domain">
                               <Requests>
                                   <Request TypeName="Request" EndpointName="GetData" Url="http://localhost:8080/" UrlWithParams="http://localhost:8080/" Method="Get" DataFormat="Json" SerializedBody="" IsMultipart="true">
                                       <ContentType Charset="UTF-8" MediaType="multipart/form-data" Boundary="DBcenmVxmKuizPPQv9DBpVK7fZOmsegQZJscXKMo" />
                                       <SerializedForm>
                                           <KeyValue Key="key-1" Value="value-1" />
                                           <KeyValue Key="key-2" Value="value-2" />
                                       </SerializedForm>
                                   </Request>
                               </Requests>
                           </Domain>
                       </Domains>
                   </SummerRestConfiguration>
                   """;

        var expected = """
                       using SummerRest.Runtime.RequestComponents;
                       using SummerRest.Runtime.Parsers;
                       using UnityEngine.Networking;
                       namespace SummerRest.Runtime.Requests
                       {
                           public static class JsonPlaceHolder
                           {
                               public static class Comments
                               {
                                   public class GetPost1Comments : SummerRest.Runtime.Requests.BaseDataRequest<GetPost1Comments>
                                   {
                                       public GetPost1Comments(): base("https://jsonplaceholder.typicode.com/comments", "https://jsonplaceholder.typicode.com/comments?postId=1", IRequestModifier<AuthRequestModifier<SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender, System.String>>.GetSingleton())
                                       {
                                           Method = HttpMethod.Get;
                                           Params.AddParamToList("postId", "1");
                                           AuthKey = "my-token-to-master-service";
                                           BodyFormat = DataFormat.Json;
                                           Init();
                                       }
                                   }
                               }
                               public static class Posts
                               {
                                   public class Get1 : SummerRest.Runtime.Requests.BaseDataRequest<Get1>
                                   {
                                       public Get1(): base("https://jsonplaceholder.typicode.com/posts/1", "https://jsonplaceholder.typicode.com/posts/1", IRequestModifier<AuthRequestModifier<SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender, System.String>>.GetSingleton())
                                       {
                                           Method = HttpMethod.Get;
                                           AuthKey = "my-token-to-master-service";
                                           BodyFormat = DataFormat.PlainText;
                                           Init();
                                       }
                                   }
                                   public class Post : SummerRest.Runtime.Requests.BaseDataRequest<Post>
                                   {
                                       public Post(): base("https://jsonplaceholder.typicode.com/posts", "https://jsonplaceholder.typicode.com/posts", IRequestModifier<AuthRequestModifier<SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender, System.String>>.GetSingleton())
                                       {
                                           Method = HttpMethod.Post;
                                           AuthKey = "my-token-to-master-service";
                                           BodyFormat = DataFormat.Json;
                                           Init();
                                       }
                                   }
                               }
                           }
                           public static class NinjaApiWithAuth
                           {
                               public class GetRandomWord : SummerRest.Runtime.Requests.BaseDataRequest<GetRandomWord>
                               {
                                   public GetRandomWord(): base("https://api.api-ninjas.com/v1/randomword", "https://api.api-ninjas.com/v1/randomword", IRequestModifier<AuthRequestModifier<SummerRestSample.NinjaApiAuthAppender, System.String>>.GetSingleton())
                                   {
                                       Method = HttpMethod.Get;
                                       AuthKey = "ninja-api-token";
                                       BodyFormat = DataFormat.Json;
                                       Init();
                                   }
                               }
                           }
                           public static class MultipartDomain
                           {
                               public class GetData : SummerRest.Runtime.Requests.BaseMultipartRequest<GetData>
                               {
                                   public GetData(): base("http://localhost:8080/", "http://localhost:8080/", null)
                                   {
                                       Method = HttpMethod.Get;
                                       ContentType = new ContentType("multipart/form-data", "UTF-8", "DBcenmVxmKuizPPQv9DBpVK7fZOmsegQZJscXKMo");
                                       MultipartFormSections.Add(new MultipartFormDataSection("key-1", "value-1"));
                                       MultipartFormSections.Add(new MultipartFormDataSection("key-2", "value-2"));
                                       Init();
                                   }
                               }
                           }
                       }
                       """;

        await RunTest(json, expected);
    }

}