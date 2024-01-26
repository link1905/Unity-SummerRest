using System;

namespace SummerRest.Editor.DataStructures
{
    [Flags]
    public enum InheritChoice
    {
        /// <summary>
        /// Will not be used in generating process
        /// </summary>
        None = 1,
        /// <summary>
        /// Use parent property
        /// </summary>
        Inherit = 2, 
        /// <summary>
        /// Append to the parent property (only used with array-like types) 
        /// </summary>
        AppendToParent = 4, 
        /// <summary>
        /// Use its own value
        /// </summary>
        Custom = 8
    }
}