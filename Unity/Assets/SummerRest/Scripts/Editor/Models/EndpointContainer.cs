using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Models
{
#if UNITY_EDITOR
    public abstract partial class EndpointContainer : ITreeBuilder
    {
        public override bool IsContainer => true;

        public List<TreeViewItemData<Endpoint>> BuildChildrenTree(int id)
        {
            var children = new List<TreeViewItemData<Endpoint>>();
            foreach (var tree in requests.Select(r => r.BuildTree(id)))
            {
                id = tree.id;
                children.Add(tree);
            }

            foreach (var tree in services.Select(service => service.BuildTree(id)))
            {
                id = tree.id;
                children.Add(tree);
            }

            return children;
        }

        public override TreeViewItemData<Endpoint> BuildTree(int id)
        {
            var children = BuildChildrenTree(id);
            if (children.Count > 0)
            {
                id = children[^1].id; //Set new id if has at least 1 child
                return new TreeViewItemData<Endpoint>(++id, this, children);
            }

            return base.BuildTree(id);
        }
        public override void Delete(bool fromParent)
        {
            foreach (var service in services)
                service.Delete(false);
            foreach (var request in requests)
                request.Delete(false);
            base.Delete(false);
        }
        public virtual void AddEndpoint<T>(T endpoint) where T : Endpoint
        {
            int count;
            switch (endpoint)
            {
                case Service service:
                    services.Add(service);
                    count = services.Count;
                    break;
                case Request request:
                    requests.Add(request);
                    count = requests.Count;
                    break;
                default:
                    return;
            }
            endpoint.Parent = this;
            if (string.IsNullOrEmpty(endpoint.EndpointName))
                endpoint.EndpointName = $"{endpoint.TypeName} {count}";
            endpoint.Domain = Domain;
        }

        public override void CacheValues()
        {
            base.CacheValues();
            foreach (var r in requests)
                r.CacheValues();
            foreach (var s in services)
                s.CacheValues();
        }
    }
#endif
    public abstract partial class EndpointContainer : Endpoint
    {
        [SerializeReference, JsonIgnore] private List<Service> services = new();
        [SerializeReference, JsonIgnore] private List<Request> requests = new();
        public List<Service> Services => services;
        public List<Request> Requests => requests;
    }
}