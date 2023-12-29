using System.IO;
using Newtonsoft.Json;
using SummerRest.Configurations;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor.Compilation;
using UnityEngine;

namespace SummerRest.Editors.Manager
{

    public static class SourceGenerator
    {
        private static string TempSourceIndicatorPath => Path.Combine(Path.GetDirectoryName(Application.dataPath)!, "Temp", 
            "SummerRestSourceGenIndicator.json");
        public static void GenerateAdditionalFile()
        {
            var domains = DomainConfigurationsManager.Instance.Domains;
            var configureJson = JsonConvert.SerializeObject(domains);
            ScriptCompilerOptions
            FileExtensions.OverwriteFile(TempSourceIndicatorPath, configureJson);
        }
    }
    // public class MyDomain
    // {
    //     public class MyService
    //     {
    //         public class MyRequest : IEndpointCaller<string>
    //         {
    //             // Gen all request fields to this
    //             // Gen string too
    //             // Support set default
    //             public IWebRequest<string> Request { get; }
    //             public string Url { get; set; }
    //             public IDictionary<string, string> Headers { get; }
    //             public IDictionary<string, string> Params { get; }
    //             public HttpMethod Method { get; set; }
    //             public int RedirectLimit { get; set; }
    //             public int TimeoutSeconds { get; set; }
    //             public IAuthData AuthData { get; set; }
    //             public ContentType ContentType { get; set; }
    //             public string BodyData { get; set; }
    //             
    //             public IEnumerator GetDetailedResponseCoroutine<TBody>(Action<IWebResponse<TBody>> doneCallback)
    //             {
    //             }
    //             public Task<IWebResponse<TBody>> GetDetailedResponseAsync<TBody>()
    //             {
    //             }
    //             public IEnumerator GetSimpleResponseCoroutine<TBody>(Action<TBody> doneCallback)
    //             {
    //             }
    //             public Task<TBody> GetSimpleResponseAsync<TBody>()
    //             {
    //             }
    //
    //         }
    //     }
    // }
    //
    // public class Samples
    // {
    //     public async void GetCaller()
    //     {
    //         var caller = EndpointManager.GetCaller<MyDomain.MyService.MyRequest>();
    //         caller.Request.TimeoutSeconds = 3;
    //         var response = await caller.GetSimpleResponseAsync<int>();
    //         
    //     }
    // }
    // //Support set default in runtime
    // public interface IEndpointCaller
    // {
    //     IEnumerator GetDetailedResponseCoroutine<TBody>(Action<IWebResponse<TBody>> doneCallback);
    //     Task<IWebResponse<TBody>> GetDetailedResponseAsync<TBody>();
    //     IEnumerator GetSimpleResponseCoroutine<TBody>(Action<TBody> doneCallback);
    //     Task<TBody> GetSimpleResponseAsync<TBody>();
    // }
    // public interface IEndpointCaller<TRequest> : IEndpointCaller, IWebRequest<TRequest>
    // {
    // }
    // public class EndpointManager
    // {
    //     public static TCaller GetCaller<TCaller>() where TCaller : IEndpointCaller
    //     {
    //     }
    // }
}