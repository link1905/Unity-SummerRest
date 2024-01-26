using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Drawers;
using UnityEngine;

namespace SummerRest.Editor.Attributes
{
    /// <summary>
    /// Decorate a <see cref="InheritOrCustomContainer{T}"/> used in <see cref="InheritOrCustomDrawer"/>
    /// </summary>
    public class InheritOrCustomAttribute : PropertyAttribute
    {
        public readonly InheritChoice Allow;
        /// <summary>
        /// Name of parent property => does not allow to use <see cref="InheritChoice.Inherit"/> and <see cref="InheritChoice.AppendToParent"/> if can not find the parent by using this name
        /// </summary>
        public readonly string ParentPropName;
        public InheritOrCustomAttribute(InheritChoice allow = InheritChoice.None | InheritChoice.Inherit | InheritChoice.Custom,
            string parentProp = "Parent")
        {
            Allow = allow;
            ParentPropName = parentProp;
        }
    }
}