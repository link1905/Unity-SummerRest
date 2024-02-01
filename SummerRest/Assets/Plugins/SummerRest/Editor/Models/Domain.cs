using SummerRest.Editor.DataStructures;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// A domain is the root node of an origin (like my-example-domain.com) <br/>
    /// It contains <see cref="Service"/> and <see cref="Request"/> but not callable
    /// </summary>
    public class Domain : EndpointContainer
    {
        /// <summary>
        /// This field is prepared for API versioning eg. https://my-test-api.com/v1 https://my-test-api.com/v2 <br/>
        /// It requires full path of an origin for each option since versioning strategies vary
        /// </summary>
        [SerializeField] private OptionsArray<string> versions;
        public override void CacheValues()
        {
            Domain = this;
            path = versions.Value;
            if (path is null)
            {
                path = "http://localhost:8080";
                versions.Values = new[] { path };
            }
            base.CacheValues();
        }
        // Active version
        [SerializeField] private string path;
        public override string Path => path;
        protected internal override string FullPath => path;
        public string ActiveVersion => path;

        public override void RemoveFormParent()
        {
        }
        public override string TypeName => nameof(Models.Domain);
    }
}