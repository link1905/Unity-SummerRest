using System;
using System.Linq;
using Newtonsoft.Json;
using SummerRest.Editor.Attributes;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Utilities;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// Base class containing an HTTP endpoint information
    /// </summary>
    public abstract class Endpoint : ScriptableObject
    {
        [field: SerializeReference][JsonIgnore]
        public Domain Domain { get; set; }
        [field: SerializeReference][JsonIgnore] 
        public Endpoint Parent { get; protected internal set; }
        
        /// <summary>
        /// This is the name of the generated class associated with this endpoint 
        /// </summary>
        [SerializeField][JsonIgnore] private string endpointName;
        public string EndpointName
        {
            get => endpointName;
            set => endpointName = value;
        }
        /// <summary>
        /// Url of the end point sequentially formed its ancestors and active origin of <see cref="Domain"/>
        /// <seealso cref="CacheValues"/> 
        /// </summary>
        [SerializeField, JsonIgnore] private string url;
        public string Url => url;
        /// <summary>
        /// Path of this endpoint, contributes to the process of creating <see cref="url"/> of this endpoint and its descendants <seealso cref="CacheValues"/>
        /// </summary>
        [SerializeField][JsonIgnore] private string path;
        public string Path
        {
            get => path;
            set => path = value;
        }
        /// <summary>
        /// Form the full path of an endpoint based on its parent excluding active origin of <see cref="Domain"/> 
        /// </summary>
        private string FullPath
        {
            get
            {
                if (Parent is null)
                    return Path;
                if (string.IsNullOrEmpty(Path))
                    return Parent.FullPath;
                return $"{Parent.FullPath}/{Path}";
            }
        }


        
        /// <summary>
        /// Points to an <see cref="SummerRest.Editor.Models.AuthContainer"/>
        /// </summary>
        [SerializeField, InheritOrCustom] 
        private InheritOrCustomContainer<AuthPointer> auth;
        public AuthContainer AuthContainer { get; private set; }

        /// <summary>
        /// Values will be appended to associated requests
        /// </summary>
        [SerializeField, JsonIgnore, InheritOrCustom(InheritChoice.Inherit | InheritChoice.None | InheritChoice.AppendToParent | InheritChoice.Custom)]
        private InheritOrCustomContainer<KeyValue[]> headers;
        public KeyValue[] Headers { get; private set; }

        /// <summary>
        /// Timeout in seconds of associated requests
        /// </summary>
        [SerializeField, JsonIgnore, InheritOrCustom]
        private InheritOrCustomContainer<int> timeoutSeconds;
        public int? TimeoutSeconds { get; private set; }

        /// <summary>
        /// Redirects limit of associated requests
        /// </summary>
        [SerializeField, JsonIgnore, InheritOrCustom]
        private InheritOrCustomContainer<int> redirectsLimit;
        public int? RedirectsLimit { get; private set; }


        [field: SerializeField] public int TreeId { get; protected set; }
        /// <summary>
        /// For building tree used in <see cref="TreeView"/>
        /// </summary>
        public virtual TreeViewItemData<Endpoint> BuildTree(int id)
        {
            TreeId = ++id;
            return new TreeViewItemData<Endpoint>(TreeId, this);
        }
        /// <summary>
        /// Delete this endpoint (and its children) and associated assets
        /// </summary>
        /// <param name="fromParent">Whether actively remove this endpoint from its parent</param>
        public virtual void Delete(bool fromParent)
        {
            this.RemoveAsset();
        }
        [JsonIgnore] public virtual bool IsContainer => false;
        public abstract string TypeName { get; }

        /// <summary>
        /// Cache the <see cref="InheritOrCustomContainer{T}"/> fields based on the <see cref="Parent"/>
        /// </summary>
        public virtual void CacheValues()
        {
            var headersCache = headers.Cache(Parent, whenInherit: p => new Present<KeyValue[]>(p.Headers != null, p.Headers),
                whenAppend: (p, t) =>
                {
                    var parent = p.Headers;
                    return parent is null ? new Present<KeyValue[]>(true, t) : new Present<KeyValue[]>(true, t.Concat(parent).ToArray());
                },
                allow: InheritChoice.Inherit | InheritChoice.None | InheritChoice.Custom | InheritChoice.AppendToParent);
            Headers = headersCache.HasValue ? headersCache.Value : null;
            
            var timeoutCache = timeoutSeconds.Cache(Parent, whenInherit: p => new Present<int>(p.TimeoutSeconds != null, p.TimeoutSeconds ?? default));
            TimeoutSeconds = timeoutCache.HasValue ? timeoutCache.Value : null;

            var redirectsLimitCache = redirectsLimit.Cache(Parent, whenInherit: p => new Present<int>(p.RedirectsLimit != null, p.RedirectsLimit ?? default));
            RedirectsLimit = redirectsLimitCache.HasValue ? redirectsLimitCache.Value : null;
            
            var authCache = auth.Cache(Parent, whenInherit: p => new Present<AuthPointer>(p.AuthContainer != null, p.AuthContainer));
            AuthContainer = authCache.HasValue ? authCache.Value : null;

            if (Domain is null)
                return;
            try
            {
                url = new Uri($"{Domain.ActiveVersion}{FullPath}").AbsoluteUri;
            }
            catch (Exception)
            {
                Debug.LogWarningFormat("{0} is not a valid URL", url);
            }
        }
        
        private void OnValidate()
        {
            CacheValues();
        }

    }
}