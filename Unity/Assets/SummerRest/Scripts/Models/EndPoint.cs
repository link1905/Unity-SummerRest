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
        [field: MemoryPackIgnore, SerializeReference, HideInInspector]
        public Domain Domain { get; protected internal set; }
        [field: MemoryPackIgnore, SerializeReference, HideInInspector]
        public EndPoint Parent { get; protected internal set; }


        [SerializeField] private string name;
        public string Name => name;

        [SerializeField, InheritOrCustom(InheritChoice.Inherit)]
        private InheritOrCustomContainer<DataFormat> dataFormat;

        public DataFormat DataFormat => dataFormat.Value;

        [SerializeField, InheritOrCustom(InheritChoice.Inherit,
             InheritChoice.Inherit | InheritChoice.None | InheritChoice.AppendToParent | InheritChoice.Custom)]
        private InheritOrCustomContainer<RequestHeader[]> headers;

        public RequestHeader[] Headers => headers.Value;

        [SerializeField, InheritOrCustom(InheritChoice.Inherit)]
        private InheritOrCustomContainer<ContentType> contentType;

        public ContentType ContentType => contentType.Value;

        [SerializeField, InheritOrCustom(InheritChoice.Inherit)]
        private InheritOrCustomContainer<int> timeoutSeconds;

        public int TimeoutSeconds => timeoutSeconds.Value;

        [SerializeField, InheritOrCustom(InheritChoice.Inherit)]
        private InheritOrCustomContainer<int> redirectsLimit;

        public int RedirectsLimit => redirectsLimit.Value;

        //[field: SerializeField] public AuthInjectorPointer AuthInjectorPointer { get; private set; }
        public string ParentPath => Parent is null ? string.Empty : Parent.Path;
        [SerializeField, HideInInspector] private string parentPath;
        public virtual string Path => Parent is null ? Name : $"{Parent.Path}/{Name}";
        public virtual string Url => $"{Domain.ActiveVersion}/{Path}";

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