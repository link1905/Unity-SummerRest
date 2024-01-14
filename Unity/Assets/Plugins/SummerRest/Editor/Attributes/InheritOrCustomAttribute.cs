using SummerRest.Editor.DataStructures;
using UnityEngine;

namespace SummerRest.Editor.Attributes
{
    public class InheritOrCustomAttribute : PropertyAttribute
    {
        public readonly InheritChoice Allow;

        public readonly string ParentPropName;
        // = new []{InheritChoice.None, InheritChoice.Inherit, InheritChoice.Custom}
        public InheritOrCustomAttribute(InheritChoice allow = InheritChoice.None | InheritChoice.Inherit | InheritChoice.Custom,
            string parentProp = "Parent")
        {
            Allow = allow;
            ParentPropName = parentProp;
        }
    }
}