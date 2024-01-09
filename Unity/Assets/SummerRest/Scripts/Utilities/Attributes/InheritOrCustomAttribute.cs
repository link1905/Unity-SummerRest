using SummerRest.Scripts.Utilities.DataStructures;
using UnityEngine;

namespace SummerRest.Scripts.Utilities.Attributes
{
    public class InheritOrCustomAttribute : PropertyAttribute
    {
        public readonly InheritChoice Allow;

        // = new []{InheritChoice.None, InheritChoice.Inherit, InheritChoice.Custom}
        public InheritOrCustomAttribute(InheritChoice allow = InheritChoice.None | InheritChoice.Inherit | InheritChoice.Custom)
        {
            Allow = allow;
        }
    }
}