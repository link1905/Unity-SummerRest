using SummerRest.Scripts.Utilities.DataStructures;
using UnityEngine;

namespace SummerRest.Scripts.Utilities.Attributes
{
    public class InheritOrCustomAttribute : PropertyAttribute
    {
        public readonly InheritChoice Default;
        public readonly InheritChoice DefaultWhenNoParent;
        public readonly InheritChoice Allow;
        public readonly string CachePropName;

        public InheritOrCustomAttribute(InheritChoice @default, string cachePropName, 
            InheritChoice allow = InheritChoice.None | InheritChoice.Inherit | InheritChoice.Custom,
            InheritChoice defaultWhenNoParent = InheritChoice.Custom)
        {
            Default = @default;
            CachePropName = cachePropName;
            Allow = allow;
            DefaultWhenNoParent = defaultWhenNoParent;
        }
    }
}