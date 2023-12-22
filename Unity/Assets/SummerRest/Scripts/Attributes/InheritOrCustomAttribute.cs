using SummerRest.DataStructures.Enums;
using UnityEngine;

namespace SummerRest.Attributes
{
    public class InheritOrCustomAttribute : PropertyAttribute
    {
        public readonly InheritChoice Default;
        public readonly InheritChoice DefaultWhenNoParent;
        public readonly InheritChoice Allow;

        public static InheritChoice All => InheritChoice.None | InheritChoice.Inherit | InheritChoice.AppendToParent |
                                           InheritChoice.Custom;
        public InheritOrCustomAttribute(InheritChoice @default, 
            InheritChoice allow = InheritChoice.None | InheritChoice.Inherit | InheritChoice.Custom,
            InheritChoice defaultWhenNoParent = InheritChoice.Custom)
        {
            Default = @default;
            Allow = allow;
            DefaultWhenNoParent = defaultWhenNoParent;
        }
    }
}