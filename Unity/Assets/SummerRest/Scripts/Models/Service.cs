using System;
using SummerRest.Scripts.Utilities.Editor;

namespace SummerRest.Models
{
    [Serializable]
    public class Service : EndpointContainer
    {
        public override void Delete(bool fromParent)
        {
            if (fromParent && Parent is EndpointContainer parent)
                parent.Services.Remove(this);
            base.Delete(fromParent);
        }
        public override string TypeName => nameof(Service);
    }
}