using System;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    public abstract class EndPointContainer : EndPoint, ISerializationCallbackReceiver
    {
        [field: SerializeField, HideInInspector] public Service[] Services { get; private set; }
        [field: SerializeField, HideInInspector] public Request[] Requests { get; private set; }
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            // foreach (var child in Services)
            //     child.Parent = this;
            // foreach (var child in Requests)
            //     child.Parent = this;
        }
    }
}