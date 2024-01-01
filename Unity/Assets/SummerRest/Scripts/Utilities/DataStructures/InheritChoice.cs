using System;

namespace SummerRest.Scripts.Utilities.DataStructures
{
    [Flags]
    public enum InheritChoice
    {
        None = 1, Inherit = 2, AppendToParent = 4, Custom = 8
    }
}