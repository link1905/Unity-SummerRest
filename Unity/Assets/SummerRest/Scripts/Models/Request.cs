using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using TypeReferences;
using UnityEngine;

namespace SummerRest.Models
{
    public interface IAuthDataRepository
    {
        IAuthData GetAuthData(string id);
    }
    public interface IAuthInjector<in T> where T : IAuthData
    {
        void InjectAuthData();
    }
    public class DefaultAuthDataRepository
    {
    }
    public class BearerTokenAuthInjector
    {
    }
    public interface IAuthData
    {
    }
    public interface IWebResponse<out TBody>
    {
        TBody Data { get; }
        string RawData { get; }
        byte[] RawDataBytes { get; }
        IEnumerable<KeyValue> Headers { get; }
        HttpStatusCode StatusCode { get; }
        string Error { get; }
        object Requester { get; }
    }
    public interface IWebRequest<TBody>
    {
        string Url { get; set; }
        IDictionary<string, string> Headers { get; }
        IDictionary<string, string> Params { get; }
        HttpMethod Method { get; set; }
        int RedirectLimit { get; set; }
        int TimeoutSeconds { get; set; }
        IAuthData AuthData { get; set; }
        ContentType ContentType { get; set; }
        TBody BodyData { get; set; }
    }

    [Serializable]
    internal class AuthConfiguration
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public TypeReference Injector { get; private set; }
        public Type InjectorType => Injector.Type;
        [field: SerializeField] public object AuthValue { get; private set; }
    }

    internal class AuthConfigurationsManager : ScriptableObject
    {
        [field: SerializeField] internal AuthConfiguration[] Configurations { get; private set; }
    }

    public partial class Request : Endpoint 
    {
        [SerializeField, JsonIgnore] private HttpMethod method;
        [SerializeField, JsonIgnore] private KeyValue[] requestParams;
        [SerializeField, JsonIgnore] private RequestBody requestBody;
        public HttpMethod Method
        {
            get => method;
            private set => method = value;
        }
        public KeyValue[] RequestParams
        {
            get => requestParams;
            private set => requestParams = value;
        }
        public RequestBody RequestBody
        {
            get => requestBody;
            private set => requestBody = value;
        }

    }

#if UNITY_EDITOR
    public partial class Request
    {
        public override void Delete(bool fromParent)
        {
            if (fromParent && Parent is EndpointContainer parent)
                parent.Requests.Remove(this);
            base.Delete(fromParent);
        }

        [SerializeField, JsonIgnore] private Response latestResponse;
        [JsonIgnore] public Response LatestResponse
        {
            get => latestResponse;
            set => latestResponse = value;
        }
        public override string TypeName => nameof(Request);
    }
#endif
    public enum RequestBodyType
    {
        PlainText = 0, Data = 1
    }
}
