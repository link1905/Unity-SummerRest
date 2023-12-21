using System;
using SummerRest.Attributes;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    public partial class EndPoint
    {
        [field: SerializeReference, HideInInspector] public Domain Domain { get; set; }
        [field: SerializeReference, HideInInspector] public EndPoint Parent { get; set; }
        [field: SerializeField] public string Name { get; private set; }
        [SerializeField, InheritOrCustom] private DataFormat dataFormat;
        [field: SerializeField] public RequestHeader[] Headers { get; private set; }
        [field: SerializeField] public ContentType ContentType { get; private set; }
        [field: SerializeField] public int TimeoutSeconds { get; private set; }
        [field: SerializeField] public int RedirectLimit { get; private set; }
        [field: SerializeField] public AuthInjectorPointer AuthInjectorPointer { get; private set; }
        public virtual string Path => $"{Parent?.Name}/{Name}";
        public virtual string Url => $"{Domain.ActiveVersion}/{Path}";
    }
}