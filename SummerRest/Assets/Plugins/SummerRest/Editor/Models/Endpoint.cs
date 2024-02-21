using System;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SummerRest.Editor.Attributes;
using SummerRest.Editor.Configurations;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// Base class containing an HTTP endpoint information
    /// </summary>
    public abstract class Endpoint : ScriptableObject, IXmlSerializable
    {
        [field: SerializeReference]
        public Domain Domain { get; set; }
        [field: SerializeReference]
        public EndpointContainer Parent { get; protected internal set; }
        
        /// <summary>
        /// This is the name of the generated class associated with this endpoint 
        /// </summary>
        [SerializeField] private string endpointName;
        public string EndpointName
        {
            get => endpointName;
            set => endpointName = value;
        }
        /// <summary>
        /// Url of the end point sequentially formed its ancestors and active origin of <see cref="Domain"/>
        /// <seealso cref="CacheValues"/> 
        /// </summary>
        [SerializeField] private string url;
        public string Url
        {
            get => url;
            set => url = value;
        }


        /// <summary>
        /// Path of this endpoint, contributes to the process of creating <see cref="url"/> of this endpoint and its descendants <seealso cref="CacheValues"/>
        /// </summary>
        public abstract string Path { get; }
        /// <summary>
        /// Form the full path of an endpoint based on its parent excluding active origin of <see cref="Domain"/> 
        /// </summary>
        protected internal virtual string FullPath
        {
            get
            {
                if (Parent is null)
                    return Path;
                var parentPath = Parent.FullPath;
                if (string.IsNullOrEmpty(Path))
                    return parentPath;
                if (string.IsNullOrEmpty(parentPath))
                    return Path;
                return $"{parentPath}/{Path}";
            }
        }


        
        /// <summary>
        /// Points to an <see cref="SummerRest.Editor.Models.AuthContainer"/>
        /// </summary>
        [SerializeField, InheritOrCustom] 
        private InheritOrCustomContainer<AuthPointer> auth;
        public AuthContainer AuthContainer { get; set; }

        /// <summary>
        /// Values will be appended to associated requests
        /// </summary>
        [SerializeField, InheritOrCustom(InheritChoice.Inherit | InheritChoice.None | InheritChoice.AppendToParent | InheritChoice.Custom)]
        private InheritOrCustomContainer<KeyValue[]> headers;
        public KeyValue[] Headers { get; set; }

        /// <summary>
        /// Timeout in seconds of associated requests
        /// </summary>
        [SerializeField, InheritOrCustom]
        private InheritOrCustomContainer<int> timeoutSeconds;
        public int? TimeoutSeconds { get; set; }

        /// <summary>
        /// Redirects limit of associated requests
        /// </summary>
        [SerializeField, InheritOrCustom]
        private InheritOrCustomContainer<int> redirectsLimit;
        public int? RedirectsLimit { get; set; }

        [SerializeField] private bool isGenerated = true;
        public bool IsSelfGenerated => isGenerated;

        public bool IsParentGenerated(bool checkSelf = false)
        {
            // Should check self but not satisfied
            if (checkSelf && !IsSelfGenerated)
                return false;
            return Parent is null || Parent.IsParentGenerated(true);
        }


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

        public virtual string Rename(string parent, int index)
        {
            var newName = $"{parent}_{TypeName}_{index}";
            AssetDatabase.RenameAsset(this.GetAssetPath(), newName);
            return newName;
        }
        public abstract void RemoveFormParent(); 
        public virtual bool IsContainer => false;
        public virtual string TypeName => nameof(Endpoint);

        public virtual void ValidateToGenerate()
        {
            if (!IsSelfGenerated)
                return;
            var showErrName = $"{EndpointName}({url})";
            var endPointClassName = EndpointName.ToClassName();
            if (string.IsNullOrEmpty(EndpointName))
                throw new Exception($"{showErrName} uses a null (or empty) generated name");
            if (this is not SummerRest.Editor.Models.Domain && endPointClassName == Parent.EndpointName.ToClassName())
                throw new Exception($"{showErrName} uses the same generated class name with its parent ({Parent.EndpointName}): {endPointClassName}");
        } 

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
            if (authCache.HasValue)
            {
                AuthContainer = authCache.Value.Cache();
                if (AuthContainer is null && SummerRestConfiguration.Instance.AuthContainers.Count != 0)
                    Debug.LogError(@$"{EndpointName}({url}) with the auth key ""{authCache.Value.AuthKey}"" points to an invalid auth container");
            }
            else
                AuthContainer = null;

            if (Domain is null)
                return;
            var fullPath = FullPath;
            try
            {
                url = Uri.EscapeUriString(fullPath);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                url = fullPath;
            }
        }
        
        private void OnValidate()
        {
            CacheValues();
        }

        public XmlSchema GetSchema() => null;
        public void ReadXml(XmlReader reader)
        {
        }
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString(nameof(TypeName), TypeName);
            writer.WriteAttributeString(nameof(EndpointName), EndpointName);
        }
    }
}