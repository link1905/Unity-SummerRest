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
    }
#endif
    public partial class Endpoint : ScriptableObject
    {
        [field: SerializeReference][JsonIgnore]
        public Domain Domain { get; set; }
        [field: SerializeReference][JsonIgnore] public Endpoint Parent { get; protected internal set; }

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

        [SerializeField, JsonIgnore, InheritOrCustom(InheritChoice.Inherit, nameof(DataFormat))]
        private InheritOrCustomContainer<DataFormat> dataFormat;
        public DataFormat DataFormat => dataFormat.Cache(whenInherit: () => Parent?.DataFormat ?? default);
        
        [SerializeField, InheritOrCustom(InheritChoice.Inherit, nameof(Authentication))] 
        private InheritOrCustomContainer<AuthPointer> authentication;
        public AuthContainer Authentication =>
            authentication.Cache(whenInherit: () => Parent is null ? null : Parent.Authentication);

        [SerializeField, JsonIgnore, InheritOrCustom(InheritChoice.Inherit, nameof(Headers),
             InheritChoice.Inherit | InheritChoice.None | InheritChoice.AppendToParent | InheritChoice.Custom)]
        private InheritOrCustomContainer<KeyValue[]> headers;
        public KeyValue[] Headers => headers.Cache(whenInherit: () => Parent.Headers, 
            whenAppend: t => Parent.Headers is not null ? Parent.Headers.Concat(t).ToArray() : t);

        [SerializeField, JsonIgnore, InheritOrCustom(InheritChoice.Inherit, nameof(ContentType))]
        private InheritOrCustomContainer<ContentType> contentType;
        public ContentType ContentType => contentType.Cache(whenInherit: () => Parent?.ContentType);

        [SerializeField, JsonIgnore, InheritOrCustom(InheritChoice.Inherit, nameof(TimeoutSeconds))]
        private InheritOrCustomContainer<int> timeoutSeconds;
        public int TimeoutSeconds => timeoutSeconds.Cache(whenInherit: () => Parent?.TimeoutSeconds ?? default);

        [SerializeField, JsonIgnore, InheritOrCustom(InheritChoice.Inherit, nameof(RedirectsLimit))]
        private InheritOrCustomContainer<int> redirectsLimit;
        public int RedirectsLimit => redirectsLimit.Cache(whenInherit: () => Parent?.RedirectsLimit ?? default);

        //[field: SerializeField] public AuthInjectorPointer AuthInjectorPointer { get; private set; }
        public virtual string FullPath => Parent is null ? Path : $"{Parent.FullPath}/{Path}";
        public virtual string Url => $"{Domain.ActiveVersion}{FullPath}";

    }
}