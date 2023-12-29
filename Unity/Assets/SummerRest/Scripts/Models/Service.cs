using System;

namespace SummerRest.Models
{
#if UNITY_EDITOR
    public partial class Service
    {
        public override void Delete(bool fromParent)
        {
            if (fromParent && Parent is EndpointContainer parent)
                parent.Services.Remove(this);
            base.Delete(fromParent);
        }
        public override string TypeName => nameof(Service);
    }
#endif
    public partial class Service : EndpointContainer
    {
    }
}