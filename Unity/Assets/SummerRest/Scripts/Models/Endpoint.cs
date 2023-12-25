using System;
using System.Linq;
using SummerRest.Attributes;
using SummerRest.DataStructures.Containers;
using SummerRest.DataStructures.Enums;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Models
{
#if UNITY_EDITOR
    using UnityEngine.UIElements;
    public interface ITreeBuilder
    {
        TreeViewItemData<Endpoint>? BuildTree(int id, string search);
    }
    public abstract partial class Endpoint : ITreeBuilder
    {
        public virtual TreeViewItemData<Endpoint>? BuildTree(int id, string search)
        {
            if (Path.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                return new TreeViewItemData<Endpoint>(++id, this);
            return null;
        }

        public virtual bool IsContainer => false;
        public abstract string TypeName { get; }
    }
#endif
    [Serializable]
    public partial class Endpoint : ScriptableObject, ISerializationCallbackReceiver
    {
        [field: SerializeReference, HideInInspector]
        public Domain Domain { get; protected internal set; }

        [field: SerializeReference, HideInInspector]
        public Endpoint Parent { get; protected internal set; }

        [SerializeField] private string endpointName;
        public string EndpointName
        {
            get => endpointName;
            set => endpointName = value;
        }

        [SerializeField, InheritOrCustom(InheritChoice.Inherit)]
        private InheritOrCustomContainer<DataFormat> dataFormat;

        public DataFormat DataFormat => dataFormat.Value;

        [SerializeField, InheritOrCustom(InheritChoice.Inherit,
             InheritChoice.Inherit | InheritChoice.None | InheritChoice.AppendToParent | InheritChoice.Custom)]
        private InheritOrCustomContainer<KeyValue[]> headers;

        public KeyValue[] Headers => headers.Value;

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
        public virtual string Path => Parent is null ? EndpointName : $"{Parent.Path}/{EndpointName}";
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
// #if UNITY_EDITOR
//             if (name != endpointName && !string.IsNullOrEmpty(endpointName))
//             {
//                 AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), Path);
//                 AssetDatabase.SaveAssets();
//             }
// #endif
        }
    }
}