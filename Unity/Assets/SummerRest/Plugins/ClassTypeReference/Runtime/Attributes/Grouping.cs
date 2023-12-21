﻿namespace TypeReferences
{
    using JetBrains.Annotations;
    using UnityEngine;

    /// <summary>
    /// Indicates how selectable types should be collated in drop-down menu.
    /// </summary>
    public enum Grouping
    {
        /// <summary>
        /// No grouping, just show type names in a list; for instance, "Some.Nested.Namespace.SpecialClass".
        /// </summary>
        [PublicAPI]
        None,

        /// <summary>
        /// Group types by namespace and show foldout menus for nested namespaces; for
        /// instance, "Some > Nested > Namespace > SpecialClass".
        /// </summary>
        [PublicAPI]
        ByNamespace,

        /// <summary>
        /// Group types by namespace; for instance, "Some.Nested.Namespace > SpecialClass".
        /// </summary>
        [PublicAPI]
        ByNamespaceFlat,

        /// <summary>
        /// Group types in the same way as Unity does for its component menu. This
        /// grouping method must only be used for <see cref="MonoBehaviour"/> types.
        /// </summary>
        [PublicAPI]
        ByAddComponentMenu
    }
}