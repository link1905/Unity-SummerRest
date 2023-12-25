using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SummerRest.Models
{
#if UNITY_EDITOR
    using UnityEngine.UIElements;
    public abstract partial class EndpointContainer : ITreeBuilder
    {
        public override bool IsContainer => true;
        public List<TreeViewItemData<Endpoint>> BuildChildrenTree(int id, string search)
        {
            var children = new List<TreeViewItemData<Endpoint>>();
            foreach (var tree in services.Select(service => service.BuildTree(id, search)).Where(s => s.HasValue))
            {
                id = tree.Value.id;
                children.Add(tree.Value);
            }
            foreach (var tree in requests.Select(r => r.BuildTree(id, search)).Where(s => s.HasValue))
            {
                id = tree.Value.id;
                children.Add(tree.Value);
            }
            return children;
        }
        public override TreeViewItemData<Endpoint>? BuildTree(int id, string search)
        {
            var children = BuildChildrenTree(id, search);
            if (children.Count > 0)
            {
                id = children[^1].id; //Set new id if has at least 1 child
                return new TreeViewItemData<Endpoint>(++id, this, children);
            }
            return base.BuildTree(id, search);
        }
    }
#endif
    [Serializable]
    public abstract partial class EndpointContainer : Endpoint
    {
        [SerializeReference, HideInInspector] private List<Service> services;
        [SerializeReference, HideInInspector] private List<Request> requests;
        public List<Service> Services
        {
            get => services;
        }
        public List<Request> Requests
        {
            get => requests;
        }
    }
}