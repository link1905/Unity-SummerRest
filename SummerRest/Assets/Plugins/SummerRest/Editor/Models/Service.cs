using UnityEngine;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// A service can contain services and requests for building api structure <br/>
    /// Service is not callable (only <see cref="Request"/>)
    /// </summary>
    public class Service : EndpointContainer
    {
        [SerializeField] private string path;
        public override string Path => path;

        public override void Delete(bool fromParent)
        {
            if (fromParent)
                Parent.Services.Remove(this);
            base.Delete(fromParent);
        }

        public override void RemoveFormParent()
        {
            if (Parent is null)
                return;
            Parent.Services.Remove(this);
        }

        public override string TypeName => nameof(Service);
    }
}