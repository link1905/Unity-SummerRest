using System;
using System.Collections.Generic;
using SummerRest.Scripts.Attributes;
using TypeReferences;
using UnityEngine;

namespace SummerRest.Scripts.Models
{
    /// <summary>
    /// Marking class for request data transfer object 
    /// </summary>
    public interface IRequestDto
    { }
    /// <summary>
    /// Marking class for response data transfer object 
    /// </summary>
    public interface IResponseModel
    { }
    /// <summary>
    /// Marking class for request param data
    /// </summary>
    public interface IRequestParamValue
    { }
    

    [Serializable]
    public enum HttpMethod
    {
        Get, Post, Put, Delete, Patch, Head, Options, Trace, Connect
    }
    [Serializable]
    public class ContentType
    {
        [field: SerializeField] public string CharSet { get; private set; }
        [field: SerializeField] public string MediaType { get; private set; }
        [field: SerializeField] public string Boundary { get; private set; }
    }
    [Serializable]
    internal enum DataFormat
    {
        Json, Xml, PlainText
    }
    [Serializable]
    internal enum AuthType
    {
        None, Advanced
    }

    #region public
    public interface IAuthDataRepository
    {
        IAuthData GetAuthData(string id);
    }
    public interface IAuthInjector<in T> where T : IAuthData
    {
        void InjectAuthData();
    }
    #endregion
    
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
    internal class DomainConfigurationsManager : ScriptableObject
    {
        [field: SerializeField] internal Domain[] Domains { get; private set; }
    }
    
    [Serializable]
    internal class AuthInjectorPointer
    {
        [field: SerializeField] public AuthType Type { get; set; }
        [field: SerializeField] public string AuthId { get; set; }
    }
    [Serializable]
    internal class ApiVersion
    {
        [field: SerializeField] public string Origin { get; private set; }
    }

    [Serializable]
    internal class EnableValue<T>
    {
        [field: SerializeField] public T Value { get; private set; }
        [field: SerializeField] public bool Enable { get; private set; }
    }
    
    [Serializable]
    internal class EndPoint
    {
        [field: SerializeField, HideInInspector] public Domain Domain { get; set; }
        [field: SerializeField, HideInInspector] public EndPoint Parent { get; set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, InheritOrCustom] public RequestHeader[] Headers { get; private set; }
        [field: SerializeField, InheritOrCustom] public DataFormat DataFormat { get; private set; }
        [field: SerializeField, InheritOrCustom] public ContentType ContentType { get; private set; }
        [field: SerializeField, InheritOrCustom] public int TimeoutSeconds { get; private set; }
        [field: SerializeField, InheritOrCustom] public int RedirectLimit { get; private set; }
        [field: SerializeField, InheritOrCustom] public AuthInjectorPointer AuthInjectorPointer { get; private set; }
        public virtual string Path => $"{Parent?.Name}/{Name}";
        public virtual string Url => $"{Domain.ActiveVersion?.Origin}/{Path}";
    }

    [Serializable]
    internal class Domain : EndPoint
    {
        [field: SerializeField] public EnableValue<ApiVersion> VersionOrigins { get; private set; }
        /// <summary>
        /// Set when change the active version in <see cref="VersionOrigins"/> 
        /// </summary>
        public ApiVersion ActiveVersion { get; private set; }
        [field: SerializeField] public Service[] Services { get; private set; }
    }
    [Serializable]
    internal class Service : EndPoint
    {
        [field: SerializeField] public EndPoint[] Children { get; private set; }
    }

    [Serializable]
    internal class RequestHeader
    {
        [field: SerializeField] public string Key { get; private set; }
        [field: SerializeField] public string Value { get; private set; }
    }

    [Serializable]
    internal class RequestParam
    {
        [field: SerializeField] public string Key { get; private set; }

        [field: SerializeReference, SerializedGenericField(typeof(bool), typeof(bool), typeof(string), typeof(float))] 
        public IRequestParamValue Value { get; private set; }
    }
    [Serializable]
    internal class Request
    {
        [field: SerializeField] public HttpMethod Method { get; private set; }
        [field: SerializeField] public RequestParam[] Params { get; private set; }
    }
}
