using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummerRest.Models
{
#if UNITY_EDITOR
    using UnityEngine.UIElements;
    public partial class EndPointContainer : ITreeBuilder
    {
        public List<TreeViewItemData<EndPoint>> BuildChildrenTree(int id)
        {
            var children = new List<TreeViewItemData<EndPoint>>();
            foreach (var tree in services.Select(service => service.BuildTree(id)))
            {
                id = tree.id;
                children.Add(tree);
            }
            foreach (var tree in requests.Select(r => r.BuildTree(id)))
            {
                id = tree.id;
                children.Add(tree);
            }
            return children;
        }
        public override TreeViewItemData<EndPoint> BuildTree(int id)
        {
            var children = BuildChildrenTree(id);
            if (children.Count > 0)
                id = children[^1].id; //Set new id if has at least 1 child
            return new TreeViewItemData<EndPoint>(++id, this, children);
        }
    }
#endif
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
    }
}