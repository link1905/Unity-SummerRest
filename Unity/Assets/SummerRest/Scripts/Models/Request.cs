using System;
using System.Collections.Generic;
using SummerRest.Attributes;
using SummerRest.DataStructures.Primitives;
using SummerRest.Models.Interfaces;
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
    public class Request : Endpoint 
    {
        [SerializeField] private HttpMethod method;
        [SerializeField] private KeyValue[] requestParams;
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
#if UNITY_EDITOR
        public override string TypeName => nameof(Request);
#endif
    }

    public enum BodyType
    {
        Text, Data
    }
    [Serializable]
    public partial class RequestBody
    {
        [SerializeField] private BodyType type;
        [SerializeField, Multiline(20)] private string text;
        [SerializeReference, SerializedGenericField(typeof(RestString))] private IRequestBodyData value;

    }
    
}
