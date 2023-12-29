using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SummerRest.Attributes;
using SummerRest.DataStructures.Containers;
using SummerRest.DataStructures.Enums;
using SummerRest.Scripts.Utilities.Common;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Models
{
#if UNITY_EDITOR
    using UnityEngine.UIElements;
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