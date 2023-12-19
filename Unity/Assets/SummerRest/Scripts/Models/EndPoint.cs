using System;
using SummerRest.Attributes;
using SummerRest.DataStructures.Primitives;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    internal class EndPoint<T> : EndPoint, ISerializationCallbackReceiver where T : EndPoint
    {
        [field: SerializeField] public T[] Children { get; private set; }
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            foreach (var child in Children)
                child.Parent = this;
        }
    }
    [Serializable]
    internal partial class EndPoint
    {
        [field: SerializeField, HideInInspector] public Domain Domain { get; set; }
        [field: SerializeField, HideInInspector] public EndPoint Parent { get; set; }
        [field: SerializeField] public string Name { get; private set; }
        [SerializeField, InheritOrCustom] private DataFormat dataFormat;
        [field: SerializeField] public RequestHeader[] Headers { get; private set; }
        [field: SerializeField] public ContentType ContentType { get; private set; }
        [field: SerializeField] public int TimeoutSeconds { get; private set; }
        [field: SerializeField] public int RedirectLimit { get; private set; }
        [field: SerializeField] public AuthInjectorPointer AuthInjectorPointer { get; private set; }
        public virtual string Path => $"{Parent?.Name}/{Name}";
        public virtual string Url => $"{Domain.ActiveVersion?.Origin}/{Path}";
    }
}