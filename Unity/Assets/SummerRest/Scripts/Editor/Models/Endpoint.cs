using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SummerRest.Editor.Utilities;
using SummerRest.Scripts.Utilities.Attributes;
using SummerRest.Scripts.Utilities.DataStructures;
using SummerRest.Scripts.Utilities.RequestComponents;
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

        [SerializeField][JsonIgnore] private string path;
        public string Path
        {
            get => path;
            set => path = value;
        }

        [SerializeField, JsonIgnore, InheritOrCustom(nameof(DataFormat))]
        private InheritOrCustomContainer<DataFormat> dataFormat;
        public DataFormat DataFormat => dataFormat.CacheValue;
        
        [SerializeField, InheritOrCustom(nameof(AuthPointer))] 
        private InheritOrCustomContainer<AuthPointer> auth;
        //[field: SerializeField] public AuthPointer AuthPointer { get; private set; }
        public AuthContainer AuthContainer => 
            auth.Choice == InheritChoice.None ? null : auth.CacheValue;

        [SerializeField, JsonIgnore, InheritOrCustom(nameof(Headers),
             InheritChoice.Inherit | InheritChoice.None | InheritChoice.AppendToParent | InheritChoice.Custom)]
        private InheritOrCustomContainer<KeyValue[]> headers;
        public KeyValue[] Headers => headers.Choice == InheritChoice.None ? null : headers.CacheValue;

        [SerializeField, JsonIgnore, InheritOrCustom(nameof(ContentType))]
        private InheritOrCustomContainer<ContentType> contentType;
        public ContentType? ContentType => 
            contentType.Choice == InheritChoice.None ? null : contentType.CacheValue;
        
        [SerializeField, JsonIgnore, InheritOrCustom(nameof(TimeoutSeconds))]
        private InheritOrCustomContainer<int> timeoutSeconds;
        public int? TimeoutSeconds => 
            timeoutSeconds.Choice == InheritChoice.None ? null : timeoutSeconds.CacheValue;

        [SerializeField, JsonIgnore, InheritOrCustom(nameof(RedirectsLimit))]
        private InheritOrCustomContainer<int> redirectsLimit;
        public int? RedirectsLimit => 
            redirectsLimit.Choice == InheritChoice.None ? null : redirectsLimit.CacheValue;
        
        //[field: SerializeField] public AuthInjectorPointer AuthInjectorPointer { get; private set; }
        public virtual string FullPath => Parent is null ? Path : $"{Parent.FullPath}/{Path}";
        public virtual string Url => $"{Domain.ActiveVersion}{FullPath}";
        
        public virtual TreeViewItemData<Endpoint> BuildTree(int id)
        {
            return new TreeViewItemData<Endpoint>(++id, this);
        }
        public virtual void Delete(bool fromParent)
        {
            this.RemoveAsset();
        }
        [JsonIgnore] public virtual bool IsContainer => false;
        public abstract string TypeName { get; }

        public virtual void CacheValues()
        {
            headers.Cache(Parent, whenInherit: p => p.Headers,
                whenAppend: (p, t) => t.Concat(p.headers.CacheValue).ToArray(),
                allow: InheritChoice.Inherit | InheritChoice.None | InheritChoice.Custom | InheritChoice.AppendToParent);
            dataFormat.Cache(Parent, whenInherit: p => p.DataFormat, 
                allow: InheritChoice.Inherit | InheritChoice.Custom | InheritChoice.AppendToParent);
            contentType.Cache(Parent, whenInherit: p => p.contentType.CacheValue);
            timeoutSeconds.Cache(Parent, whenInherit: p => p.timeoutSeconds.CacheValue);
            redirectsLimit.Cache(Parent, whenInherit: p => p.redirectsLimit.CacheValue);
            auth.Cache(Parent, whenInherit: p => p.auth.CacheValue);
        }
        
        private void OnValidate()
        {
            CacheValues();
        }

    }
}