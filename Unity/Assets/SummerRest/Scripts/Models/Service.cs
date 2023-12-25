using System;

namespace SummerRest.Models
{
    [Serializable]
    public class Service : EndpointContainer
    {
        public override string TypeName => nameof(Service);
    }
}