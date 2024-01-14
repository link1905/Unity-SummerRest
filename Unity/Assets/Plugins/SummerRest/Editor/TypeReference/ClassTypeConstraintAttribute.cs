using System;
using UnityEngine;

namespace SummerRest.Editor.TypeReference
{
    /// <summary>
    /// Base class for class selection constraints that can be applied when selecting
    /// a <see cref="ClassTypeReference"/> with the Unity inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ClassTypeConstraintAttribute : PropertyAttribute
    {
        /// <summary>
        /// Gets or sets grouping of selectable classes. Defaults to <see cref="ClassGrouping.ByNamespaceFlat"/>
        /// unless explicitly specified.
        /// </summary>
        public ClassGrouping Grouping { get; set; } = ClassGrouping.ByNamespaceFlat;
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassTypeConstraintAttribute"/> class.
        /// </summary>
        /// <param name="interfaceType">Type of interface that selectable classes must implement.</param>
        public ClassTypeConstraintAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
        /// <summary>
        /// Gets the type of interface that selectable classes must implement.
        /// </summary>
        public Type InterfaceType { get; private set; }
        public bool IsConstraintSatisfied(Type type)
        {
            return !type.IsAbstract && !type.IsInterface && InterfaceType.IsAssignableFrom(type);
        }
    }
}