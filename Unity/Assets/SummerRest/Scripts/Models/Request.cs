using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using SummerRest.Attributes;
using SummerRest.DataStructures.Containers;
using SummerRest.DataStructures.Primitives;
using SummerRest.Models.Interfaces;
using SummerRest.Scripts.Utilities.Editor;
using TypeReferences;
using UnityEngine;
using UnityEngine.Serialization;

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

    public interface IWebRequest<TBody>
    {
        Uri Uri { get; set; }
        string Url { get; set; }
        IDictionary<string, object> Params { get; }
        IDictionary<string, string> Headers { get; }
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

    [Serializable]
    public class Response
    {
        [SerializeField] private HttpStatusCode statusCode = HttpStatusCode.OK;
        public HttpStatusCode StatusCode => statusCode;
        [SerializeField] private KeyValue[] headers;
        public KeyValue[] Header
        {
            get => headers;
            set => headers = value;
        }
        [SerializeField] private string body;
        public string Body => body;
    }
    
    [Serializable]
    public partial class Request : Endpoint 
    {
        [SerializeField] private HttpMethod method;
        [SerializeField] private KeyValue[] requestParams;
        [SerializeField] private RequestBody requestBody;
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

    }

#if UNITY_EDITOR
    public partial class Request : Endpoint
    {
        public override void Delete(bool fromParent)
        {
            if (fromParent && Parent is EndpointContainer parent)
                parent.Requests.Remove(this);
            base.Delete(fromParent);
        }

        [SerializeField] private Response latestResponse;

        public Response LatestResponse
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
    [Serializable]
    public class RequestBody
    {
        [SerializeField] private RequestBodyType type;
        [SerializeField] private string text;
        [SerializeField] private RequestBodyContainer bodyContainer;
        public string SerializedData => type == RequestBodyType.PlainText ? text : bodyContainer.SerializedData;
        
        [Serializable]
        public class RequestBodyContainer : InterfaceContainer<IRequestBodyData>
        {
            [SerializeField, Inherits(typeof(IRequestBodyData))] private TypeReference typeReference;
            public override Type Type => typeReference?.Type is null ? null : Type.GetType(typeReference.TypeNameAndAssembly);
        }
    }
}
