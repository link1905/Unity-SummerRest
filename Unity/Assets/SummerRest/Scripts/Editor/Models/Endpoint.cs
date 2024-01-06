using System.Linq;
using Newtonsoft.Json;
using SummerRest.Editor.Utilities;
using SummerRest.Scripts.Utilities.Attributes;
using SummerRest.Scripts.Utilities.DataStructures;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Models
{
#if UNITY_EDITOR
    public interface ITreeBuilder
    {
        TreeViewItemData<Endpoint> BuildTree(int id);
    }
    public abstract partial class Endpoint : ITreeBuilder
    {
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
            // Encountering a problem when trying to wrap AppendToParent as func parameter to Cache method
            // So I need to show it here
            // const InheritChoice defaultAllow = InheritChoice.Inherit | InheritChoice.None | InheritChoice.Custom;
            // const InheritChoice defaultSetWhenNoParent = InheritChoice.Custom;
            // if (headers.Choice == InheritChoice.AppendToParent)
            //     Headers = headers.Value.Concat(Parent.Headers).ToArray();
            // else
            //     Headers = headers.Cache(Parent, whenInherit: p => p.Headers);
            Headers = headers.Cache(Parent, whenInherit: p => p.Headers,
                whenAppend: (p, t) => t.Concat(p.Headers).ToArray(),
                allow: InheritChoice.Inherit | InheritChoice.None | InheritChoice.Custom | InheritChoice.AppendToParent);
            DataFormat = dataFormat.Cache(Parent, whenInherit: p => p.DataFormat);
            ContentType = contentType.Cache(Parent, whenInherit: p => p.ContentType);
            TimeoutSeconds = timeoutSeconds.Cache(Parent, whenInherit: p => p.TimeoutSeconds);
            RedirectsLimit = redirectsLimit.Cache(Parent, whenInherit: p => p.RedirectsLimit);
            AuthPointer = auth.Cache(Parent, whenInherit: p => p.AuthPointer);
        }
        
        private void OnValidate()
        {
            CacheValues();
        }
    }
#endif
    public partial class Endpoint : ScriptableObject
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
        [field: SerializeField] public DataFormat DataFormat { get; private set; }
        
        [SerializeField, InheritOrCustom(nameof(AuthPointer))] 
        private InheritOrCustomContainer<AuthPointer> auth;
        [field: SerializeField] public AuthPointer AuthPointer { get; private set; }

        [SerializeField, JsonIgnore, InheritOrCustom(nameof(Headers),
             InheritChoice.Inherit | InheritChoice.None | InheritChoice.AppendToParent | InheritChoice.Custom)]
        private InheritOrCustomContainer<KeyValue[]> headers;
        [field: SerializeField] public KeyValue[] Headers { get; private set; }

        [SerializeField, JsonIgnore, InheritOrCustom(nameof(ContentType))]
        private InheritOrCustomContainer<ContentType> contentType;
        [field: SerializeField] public ContentType ContentType { get; private set; }

        [SerializeField, JsonIgnore, InheritOrCustom(nameof(TimeoutSeconds))]
        private InheritOrCustomContainer<int> timeoutSeconds;
        [field: SerializeField] public int TimeoutSeconds { get; private set; }

        [SerializeField, JsonIgnore, InheritOrCustom(nameof(RedirectsLimit))]
        private InheritOrCustomContainer<int> redirectsLimit;
        [field: SerializeField] public int RedirectsLimit { get; private set; }

        //[field: SerializeField] public AuthInjectorPointer AuthInjectorPointer { get; private set; }
        public virtual string FullPath => Parent is null ? Path : $"{Parent.FullPath}/{Path}";
        public virtual string Url => $"{Domain.ActiveVersion}{FullPath}";

    }
}