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
                                               public const string Header1 = "header1";
                                               public const string _1 = "1";
                                           }
                                           public static class Params
                                           {
                                               public const string Param1 = "param1";
                                           }
                                           public static class MultipartFormSections
                                           {
                                           }
                                       }
                                       public Request1(): base("http://domain1.com/service1/get/1", "http://domain1.com/service1/get/1?param1=param1-value", "http://domain1.com/service1/get/{0}", new string[]{"format-value-1"}, IRequestModifier<AuthRequestModifier<SummerRest.Runtime.Authenticate.Appenders.BearerTokenAuthAppender, System.String>>.GetSingleton())
                                       {
                                           Method = HttpMethod.Get;
                                           TimeoutSeconds = 0;
                                           ContentType = new ContentType("text/plain", "UTF-8", "");
                                           Headers.TryAdd(Keys.Headers.Header1, "value1");
                                           Headers.TryAdd(Keys.Headers._1, "value2");
                                           Params.AddParamToList(Keys.Params.Param1, "param1-value");
                                           AuthKey = SummerRest.Runtime.Authenticate.AuthKeys.MyTokenToMasterService;
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
                       <AuthKeys>
                           <string>dummy-json-token</string>
                       </AuthKeys>
                       <Domains>
                           <Domain TypeName="Domain" EndpointName="DummyJson">
                               <Services>
                                   <Service TypeName="Service" EndpointName="Products">
                                       <Requests>
                                           <Request TypeName="Request" EndpointName="GetProduct" Url="https://dummyjson.com/products/1" UrlFormat="https://dummyjson.com/products/{0}" UrlWithParams="https://dummyjson.com/products/1" Method="Get" DataFormat="Json" SerializedBody="" IsMultipart="false">
                                               <UrlFormatContainers>
                                                   <KeyValue Key="productId" Value="1" />
                                               </UrlFormatContainers>
                                           </Request>
                                           <Request TypeName="Request" EndpointName="SearchProduct" Url="https://dummyjson.com/products/search" UrlFormat="" UrlWithParams="https://dummyjson.com/products/search?q=phone" Method="Get" DataFormat="Json" SerializedBody="" IsMultipart="false">
                                               <RequestParams>
                                                   <KeyValue Key="q" Value="phone" />
                                               </RequestParams>
                                           </Request>
                                           <Request TypeName="Request" EndpointName="AddProductRawText" Url="https://dummyjson.com/products/add" UrlFormat="" UrlWithParams="https://dummyjson.com/products/add" Method="Post" DataFormat="Json" SerializedBody="{&#xD;&#xA;    &quot;id&quot;: 101,&#xD;&#xA;    &quot;title&quot;: &quot;my product&quot;,&#xD;&#xA;    &quot;description&quot;: &quot;my description&quot;&#xD;&#xA;}" IsMultipart="false">
                                               <ContentType Charset="UTF-8" MediaType="application/json" />
                                           </Request>
                                           <Request TypeName="Request" EndpointName="AddProductData" Url="https://dummyjson.com/products/add" UrlFormat="" UrlWithParams="https://dummyjson.com/products/add" Method="Post" DataFormat="Json" SerializedBody="{&quot;id&quot;:103,&quot;title&quot;:&quot;my product&quot;,&quot;description&quot;:&quot;it is a wonderful product&quot;}" IsMultipart="false">
                                               <ContentType Charset="UTF-8" MediaType="application/json" />
                                           </Request>
                                       </Requests>
                                   </Service>
                                   <Service TypeName="Service" EndpointName="Auth">
                                       <Requests>
                                           <Request TypeName="Request" EndpointName="Login" Url="https://dummyjson.com/auth/login" UrlFormat="" UrlWithParams="https://dummyjson.com/auth/login" Method="Post" DataFormat="Json" SerializedBody="{    &#xD;&#xA;    &quot;username&quot;: &quot;atuny0&quot;,&#xD;&#xA;    &quot;password&quot;: &quot;9uQFF1Lh&quot;&#xD;&#xA;}" IsMultipart="false">
                                               <ContentType Charset="UTF-8" MediaType="application/json" />
                                           </Request>
                                           <Request TypeName="Request" EndpointName="GetUser" Url="https://dummyjson.com/auth/me" UrlFormat="" UrlWithParams="https://dummyjson.com/auth/me" Method="Get" DataFormat="Json" SerializedBody="" IsMultipart="false">
                                               <AuthContainer AuthKey="dummy-json-token" AppenderType="SummerRestSample.DummyJsonApiAuthAppender" AuthDataType="System.String" />
                                           </Request>
                                       </Requests>
                                   </Service>
                               </Services>
                               <Requests>
                                   <Request TypeName="Request" EndpointName="Multipart" Url="https://dummyjson.com/test-multipart" UrlFormat="" UrlWithParams="https://dummyjson.com/test-multipart" Method="Post" DataFormat="Json" SerializedBody="" IsMultipart="true">
                                       <ContentType Charset="UTF-8" MediaType="multipart/form-data" Boundary="JUmGAnJMYjAWqepbCElDlxBInI8xZZuKFzTG7DNi" />
                                       <SerializedForm>
                                           <KeyValue Key="text" Value="my text value" />
                                           <KeyValue Key="file" />
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
                       namespace SummerRest.Runtime.Authenticate
                       {
                           public static class AuthKeys
                           {
                               public const string DummyJsonToken = "dummy-json-token";
                           }
                       }
                       namespace SummerRest.Runtime.Requests
                       {
                           public static class DummyJson
                           {
                               public sealed class Multipart : SummerRest.Runtime.Requests.BaseMultipartRequest<Multipart>
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
                                       }
                                       public static class MultipartFormSections
                                       {
                                           public const string Text = "text";
                                           public const string File = "file";
                                       }
                                   }
                                   public Multipart(): base("https://dummyjson.com/test-multipart", "https://dummyjson.com/test-multipart", null, System.Array.Empty<string>(), null)
                                   {
                                       Method = HttpMethod.Post;
                                       ContentType = new ContentType("multipart/form-data", "UTF-8", "JUmGAnJMYjAWqepbCElDlxBInI8xZZuKFzTG7DNi");
                                       MultipartFormSections.Add(new MultipartFormDataSection(Keys.MultipartFormSections.Text, "my text value"));
                                       Init();
                                   }
                               }
                               public static class Products
                               {
                                   public sealed class GetProduct : SummerRest.Runtime.Requests.BaseDataRequest<GetProduct>
                                   {
                                       public static class Keys
                                       {
                                           public static class UrlFormat
                                           {
                                               public const int ProductId = 0;
                                           }
                                           public static class Headers
                                           {
                                           }
                       
                                           public static class Params
                                           {
                                           }
                                           public static class MultipartFormSections
                                           {
                                           }
                                       }
                                       public GetProduct(): base("https://dummyjson.com/products/1", "https://dummyjson.com/products/1", "https://dummyjson.com/products/{0}", new string[]{"1"}, null)
                                       {
                                           Method = HttpMethod.Get;
                                           BodyFormat = DataFormat.Json;
                                           Init();
                                       }
                                   }
                                   public sealed class SearchProduct : SummerRest.Runtime.Requests.BaseDataRequest<SearchProduct>
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
                                               public const string Q = "q";
                                           }
                                           public static class MultipartFormSections
                                           {
                                           }
                                       }
                                       public SearchProduct(): base("https://dummyjson.com/products/search", "https://dummyjson.com/products/search?q=phone", null, System.Array.Empty<string>(), null)
                                       {
                                           Method = HttpMethod.Get;
                                           Params.AddParamToList(Keys.Params.Q, "phone");
                                           BodyFormat = DataFormat.Json;
                                           Init();
                                       }
                                   }
                                   public sealed class AddProductRawText : SummerRest.Runtime.Requests.BaseDataRequest<AddProductRawText>
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
                                           }
                       
                                           public static class MultipartFormSections
                                           {
                                           }
                                       }
                                       public AddProductRawText(): base("https://dummyjson.com/products/add", "https://dummyjson.com/products/add", null, System.Array.Empty<string>(), null)
                                       {
                                           Method = HttpMethod.Post;
                                           ContentType = new ContentType("application/json", "UTF-8", "");
                                           BodyFormat = DataFormat.Json;
                                           InitializedSerializedBody = @"{
                           ""id"": 101,
                           ""title"": ""my product"",
                           ""description"": ""my description""
                       }";
                                           Init();
                                       }
                                   }
                                   public sealed class AddProductData : SummerRest.Runtime.Requests.BaseDataRequest<AddProductData>
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
                                           }
                       
                                           public static class MultipartFormSections
                                           {
                                           }
                                       }
                                       public AddProductData(): base("https://dummyjson.com/products/add", "https://dummyjson.com/products/add", null, System.Array.Empty<string>(), null)
                                       {
                                           Method = HttpMethod.Post;
                                           ContentType = new ContentType("application/json", "UTF-8", "");
                                           BodyFormat = DataFormat.Json;
                                           InitializedSerializedBody = @"{""id"":103,""title"":""my product"",""description"":""it is a wonderful product""}";
                                           Init();
                                       }
                                   }
                               }
                               public static class Auth
                               {
                                   public sealed class Login : SummerRest.Runtime.Requests.BaseDataRequest<Login>
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
                                           }
                       
                                           public static class MultipartFormSections
                                           {
                                           }
                                       }
                                       public Login(): base("https://dummyjson.com/auth/login", "https://dummyjson.com/auth/login", null, System.Array.Empty<string>(), null)
                                       {
                                           Method = HttpMethod.Post;
                                           ContentType = new ContentType("application/json", "UTF-8", "");
                                           BodyFormat = DataFormat.Json;
                                           InitializedSerializedBody = @"{    
                           ""username"": ""atuny0"",
                           ""password"": ""9uQFF1Lh""
                       }";
                                           Init();
                                       }
                                   }
                                   public sealed class GetUser : SummerRest.Runtime.Requests.BaseDataRequest<GetUser>
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
                                           }
                                           public static class MultipartFormSections
                                           {
                                           }
                                       }
                                       public GetUser(): base("https://dummyjson.com/auth/me", "https://dummyjson.com/auth/me", null, System.Array.Empty<string>(), IRequestModifier<AuthRequestModifier<SummerRestSample.DummyJsonApiAuthAppender, System.String>>.GetSingleton())
                                       {
                                           Method = HttpMethod.Get;
                                           AuthKey = SummerRest.Runtime.Authenticate.AuthKeys.DummyJsonToken;
                                           BodyFormat = DataFormat.Json;
                                           Init();
                                       }
                                   }
                               }
                           }
                       }
                       """;

        await RunTest(json, expected);
    }

}