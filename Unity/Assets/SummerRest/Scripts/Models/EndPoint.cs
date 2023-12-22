using System;
using System.Linq;
using MemoryPack;
using SummerRest.Attributes;
using SummerRest.DataStructures.Containers;
using SummerRest.DataStructures.Enums;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    [MemoryPackable]
    public partial class EndPoint : ISerializationCallbackReceiver
    {
        [field: SerializeReference, HideInInspector]
        [MemoryPackIgnore]
        public Domain Domain { get; protected internal set; }
        
        [field: SerializeReference, HideInInspector]
        [MemoryPackIgnore] 
        public EndPoint Parent { get; protected internal set; }


        [SerializeField, MemoryPackInclude] private string name;
        [MemoryPackIgnore] public string Name => name;

        [SerializeField, MemoryPackInclude, InheritOrCustom(InheritChoice.Inherit)]
        private InheritOrCustomContainer<DataFormat> dataFormat;
        [MemoryPackIgnore] public DataFormat DataFormat => dataFormat.Value;

        [SerializeField, MemoryPackInclude, InheritOrCustom(InheritChoice.Inherit,
             InheritChoice.Inherit | InheritChoice.None | InheritChoice.AppendToParent | InheritChoice.Custom)]
        private InheritOrCustomContainer<KeyValue[]> headers;
        [MemoryPackIgnore] public KeyValue[] Headers => headers.Value;

        [SerializeField, MemoryPackInclude, InheritOrCustom(InheritChoice.Inherit)]
        private InheritOrCustomContainer<ContentType> contentType;
        [MemoryPackIgnore] public ContentType ContentType => contentType.Value;

        [SerializeField, MemoryPackInclude, InheritOrCustom(InheritChoice.Inherit)]
        private InheritOrCustomContainer<int> timeoutSeconds;
        [MemoryPackIgnore] public int TimeoutSeconds => timeoutSeconds.Value;

        [SerializeField, MemoryPackInclude, InheritOrCustom(InheritChoice.Inherit)]
        private InheritOrCustomContainer<int> redirectsLimit;
        [MemoryPackIgnore] public int RedirectsLimit => redirectsLimit.Value;

        //[field: SerializeField] public AuthInjectorPointer AuthInjectorPointer { get; private set; }
        [MemoryPackIgnore] public string ParentPath => Parent is null ? string.Empty : Parent.Path;
        [SerializeField, HideInInspector, MemoryPackIgnore] private string parentPath;
        [MemoryPackIgnore] public virtual string Path => Parent is null ? Name : $"{Parent.Path}/{Name}";
        [MemoryPackIgnore] public virtual string Url => $"{Domain.ActiveVersion}/{Path}";

        public virtual void OnBeforeSerialize()
        {
            if (dataFormat is null)
                return;
            dataFormat.Cache(whenInherit: () => Parent?.DataFormat ?? default);
            headers.Cache(whenInherit: () => Parent?.Headers, whenAppend: (t) => Parent.Headers.Concat(t).ToArray());
            contentType.Cache(whenInherit: () => Parent?.ContentType);
            timeoutSeconds.Cache(whenInherit: () => Parent?.TimeoutSeconds ?? default);
            redirectsLimit.Cache(whenInherit: () => Parent?.RedirectsLimit ?? default);
            parentPath = ParentPath;
        }

        public virtual void OnAfterDeserialize()
        {
        }
    }
}