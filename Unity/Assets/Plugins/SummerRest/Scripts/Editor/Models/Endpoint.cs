using System;
using System.Linq;
using Newtonsoft.Json;
using SummerRest.Editor.Attributes;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Utilities;
using SummerRest.Utilities.RequestComponents;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Models
{
    public interface ITreeBuilder
    {
        TreeViewItemData<Endpoint> BuildTree(int id);
    }
    public abstract class Endpoint : ScriptableObject
    {
        [field: SerializeReference][JsonIgnore]
        public Domain Domain { get; set; }
        [field: SerializeReference][JsonIgnore] 
        public Endpoint Parent { get; protected internal set; }

        [SerializeField][JsonIgnore] private string endpointName;
        
        public string EndpointName
        {
            get => endpointName;
            set => endpointName = value;
        }
        [SerializeField, JsonIgnore] private string url;
        public string Url => url;
        
        [SerializeField][JsonIgnore] private string path;
        public string Path
        {
            get => path;
            set => path = value;
        }

        [SerializeField, JsonIgnore, InheritOrCustom]
        private InheritOrCustomContainer<DataFormat> dataFormat;
        public DataFormat DataFormat { get; private set; }
        
        [SerializeField, InheritOrCustom] 
        private InheritOrCustomContainer<AuthPointer> auth;
        //[field: SerializeField] public AuthPointer AuthPointer { get; private set; }
        public AuthContainer AuthContainer { get; private set; }

        [SerializeField, JsonIgnore, InheritOrCustom(InheritChoice.Inherit | InheritChoice.None | InheritChoice.AppendToParent | InheritChoice.Custom)]
        private InheritOrCustomContainer<KeyValue[]> headers;
        public KeyValue[] Headers { get; private set; }

        [SerializeField, JsonIgnore, InheritOrCustom]
        private InheritOrCustomContainer<ContentType> contentType;
        public ContentType? ContentType { get; private set; }
        
        [SerializeField, JsonIgnore, InheritOrCustom]
        private InheritOrCustomContainer<int> timeoutSeconds;
        public int? TimeoutSeconds { get; private set; }

        [SerializeField, JsonIgnore, InheritOrCustom]
        private InheritOrCustomContainer<int> redirectsLimit;
        public int? RedirectsLimit { get; private set; }

        //[field: SerializeField] public AuthInjectorPointer AuthInjectorPointer { get; private set; }
        private string FullPath
        {
            get
            {
                if (Parent is null)
                    return Path;
                if (string.IsNullOrEmpty(Path))
                    return Parent.FullPath;
                return $"{Parent.FullPath}/{Path}";
            }
        }

        [field: SerializeField] public int TreeId { get; protected set; }
        public virtual TreeViewItemData<Endpoint> BuildTree(int id)
        {
            TreeId = ++id;
            return new TreeViewItemData<Endpoint>(TreeId, this);
        }
        public virtual void Delete(bool fromParent)
        {
            this.RemoveAsset();
        }
        [JsonIgnore] public virtual bool IsContainer => false;
        public abstract string TypeName { get; }

        public virtual void CacheValues()
        {
            var headersCache = headers.Cache(Parent, whenInherit: p => new Present<KeyValue[]>(p.Headers != null, p.Headers),
                whenAppend: (p, t) =>
                {
                    var parent = p.Headers;
                    return parent is null ? new Present<KeyValue[]>(true, t) : new Present<KeyValue[]>(true, t.Concat(parent).ToArray());
                },
                allow: InheritChoice.Inherit | InheritChoice.None | InheritChoice.Custom | InheritChoice.AppendToParent);
            Headers = headersCache.HasValue ? headersCache.Value : null;
            
            DataFormat = dataFormat.Cache(Parent, whenInherit: p => new Present<DataFormat>(true, p.DataFormat), 
                allow: InheritChoice.Inherit | InheritChoice.Custom | InheritChoice.AppendToParent).Value;
            
            var contentTypeCache = contentType.Cache(Parent, whenInherit: p => new Present<ContentType>(p.ContentType != null, p.ContentType ?? default));
            ContentType = contentTypeCache.HasValue ? contentTypeCache.Value : null;
            
            var timeoutCache = timeoutSeconds.Cache(Parent, whenInherit: p => new Present<int>(p.TimeoutSeconds != null, p.TimeoutSeconds ?? default));
            TimeoutSeconds = timeoutCache.HasValue ? timeoutCache.Value : null;

            var redirectsLimitCache = redirectsLimit.Cache(Parent, whenInherit: p => new Present<int>(p.RedirectsLimit != null, p.RedirectsLimit ?? default));
            RedirectsLimit = redirectsLimitCache.HasValue ? redirectsLimitCache.Value : null;
            
            var authCache = auth.Cache(Parent, whenInherit: p => new Present<AuthPointer>(p.AuthContainer != null, p.AuthContainer));
            AuthContainer = authCache.HasValue ? authCache.Value : null;

            if (Domain is null)
                return;
            try
            {
                url = new Uri($"{Domain.ActiveVersion}{FullPath}").AbsoluteUri;
            }
            catch (Exception)
            {
                Debug.LogWarningFormat("{0} is not a valid URL", url);
            }
        }
        
        private void OnValidate()
        {
            CacheValues();
        }

    }
}