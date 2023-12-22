using System;
using System.Collections.Generic;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    public abstract partial class EndPointContainer : EndPoint
    {
        [SerializeReference, HideInInspector] private List<Service> services;
        [SerializeReference, HideInInspector] private List<Request> requests;
        public List<Service> Services { get => services;
            protected set => services = value;
        }
        public List<Request> Requests { get => requests;
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