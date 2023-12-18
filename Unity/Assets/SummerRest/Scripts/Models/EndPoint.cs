using System;
using SummerRest.Scripts.Attributes;
using UnityEngine;

namespace SummerRest.Scripts.Models
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
}