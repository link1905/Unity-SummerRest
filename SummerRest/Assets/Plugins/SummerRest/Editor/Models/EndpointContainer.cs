using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SummerRest.Editor.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// Endpoint class that be able to contain other endpoints and do stuff recursively <br/>
    /// Separate services and requests to easily build editor views and generate source 
    /// </summary>
    public abstract  class EndpointContainer : Endpoint
    {
        [SerializeReference] private List<Service> services = new();
        [SerializeReference] private List<Request> requests = new();
        public List<Service> Services
        {
            get => services;
            set => services = value;
        }
        public List<Request> Requests
        {
            get => requests;
            set => requests = value;
        }

        public override bool IsContainer => true;

        /// <summary>
        /// Build tree for children
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Children tree</returns>
        private List<TreeViewItemData<Endpoint>> BuildChildrenTree(int id)
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
                //Set new id if has at least 1 child
                TreeId = children[^1].id + 1;
                return new TreeViewItemData<Endpoint>(TreeId, this, children);
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
            endpoint.RemoveFormParent();
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
            endpoint.Domain = Domain;
            if (string.IsNullOrEmpty(endpoint.EndpointName))
                endpoint.EndpointName = $"{endpoint.TypeName} {count}";
            endpoint.MakeDirty();
            this.MakeDirty();
        }

        public override string Rename(string parent, int index)
        {
            var newName = base.Rename(parent, index);
            for (int i = 0; i < requests.Count; i++)
                requests[i].Rename(newName, i);
            for (int i = 0; i < services.Count; i++)
                services[i].Rename(newName, i);
            return newName;
        }
   
        private void ForceChildCache(IEnumerable<Endpoint> endpoints)
        {
            foreach (var endpoint in endpoints)
            {
                endpoint.Parent = this;
                endpoint.Domain = Domain;
                endpoint.CacheValues();
            }
        }
        
        public override void ValidateToGenerate()
        {
            if (!IsSelfGenerated)
                return;
            base.ValidateToGenerate();
            foreach (var request in requests)
                request.ValidateToGenerate();
            foreach (var service in services)
                service.ValidateToGenerate();
        } 
        
        public override void CacheValues()
        {
            base.CacheValues();
            // Recursively call children's caching
            ForceChildCache(requests);
            ForceChildCache(services);
        }

        public bool IsChildOf(Endpoint dragEndpoint)
        {
            var parent = Parent;
            while (parent is not null)
            {
                if (parent == dragEndpoint)
                    return true;
                parent = parent.Parent;
            }
            return false;
        }
        
        public override void WriteXml(XmlWriter writer)
        {
            if (!IsSelfGenerated)
                return;
            base.WriteXml(writer);
            writer.WriteArray(nameof(Services), nameof(Service), services);
            writer.WriteArray(nameof(Requests), nameof(Request), requests);
        }
    }
}