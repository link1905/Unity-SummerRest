using SummerRest.Scripts.Utilities.DataStructures;
using UnityEngine;

namespace SummerRest.Scripts.Utilities.Attributes
{
    public class InheritOrCustomAttribute : PropertyAttribute
    {
        public readonly InheritChoice Allow;
        public readonly string CachePropName;

        // = new []{InheritChoice.None, InheritChoice.Inherit, InheritChoice.Custom}
        public InheritOrCustomAttribute(string cachePropName, 
            InheritChoice allow = InheritChoice.None | InheritChoice.Inherit | InheritChoice.Custom)
        {
            CachePropName = cachePropName;
            Allow = allow;
        }
    }
}