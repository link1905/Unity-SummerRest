using System;
using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(Service))]
    [MemoryPackUnion(1, typeof(Domain))]
    public abstract partial class EndPointContainer : EndPoint
    {
        [SerializeReference, HideInInspector, MemoryPackInclude] private List<Service> services;
        [SerializeReference, HideInInspector, MemoryPackInclude] private List<Request> requests;
        [MemoryPackIgnore] public List<Service> Services { get => services;
            protected set => services = value;
        }
        [MemoryPackIgnore] public List<Request> Requests { get => requests;
            protected set => requests = value;
        }
        public override void OnBeforeSerialize()
        {
            if (Services is not { Count: > 0 }) 
                return;
            foreach (var service in Services)
                service.Parent = this;
            foreach (var request in Requests)
                request.Parent = this;
            base.OnBeforeSerialize();
        }

        public override void OnAfterDeserialize()
        {

        }
    }
}