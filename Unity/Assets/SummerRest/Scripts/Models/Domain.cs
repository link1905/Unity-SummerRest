using System;
using MemoryPack;
using SummerRest.DataStructures.Containers;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    [MemoryPackable]
    public partial class Domain : EndPointContainer
    {
        [SerializeField] private OptionsArray<string> versions;
        public string ActiveVersion => versions.Value;
        public override void OnBeforeSerialize()
        {
            if (Services is not { Count: > 0 }) 
                return;
            foreach (var service in Services)
                service.Domain = this;
            foreach (var request in Requests)
                request.Domain = this;
            base.OnBeforeSerialize();
        }
    }
}