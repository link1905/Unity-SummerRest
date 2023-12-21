﻿namespace TypeReferences
{
    using System;
    using System.Linq;
    using JetBrains.Annotations;
    using SolidUtilities;
    using UnityEngine;

    /// <summary>
    /// Attribute for class selection constraints that can be applied when selecting
    /// a <see cref="TypeReference"/> with the Unity inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeOptionsAttribute : PropertyAttribute
    {
        /// <summary>
        /// Gets or sets grouping of selectable types. Defaults to <see><cref>Grouping.ByNamespaceFlat</cref></see>
        /// unless explicitly specified.
        /// </summary>
        [PublicAPI] public Grouping Grouping = Grouping.ByNamespaceFlat;

        /// <summary>
        /// Shows the (None) element in a dropdown so that the user can choose it.
        /// Note that if this option is disabled, the type can still be null by default or if set through code.
        /// Defaults to <see langword="true"/> unless explicitly specified.
        /// </summary>
        [PublicAPI] public bool ShowNoneElement = true;
        
        /// <summary>
        /// Use <see cref="ShowNoneElement"/> instead.
        /// </summary>
        [PublicAPI, Obsolete("Use ShowNoneElement instead.")] public bool ExcludeNone = false;

        /// <summary>Includes additional types in the drop-down list.</summary>
        [PublicAPI] public Type[] IncludeTypes;

        /// <summary>Excludes some of the types from the drop-down list.</summary>
        [PublicAPI] public Type[] ExcludeTypes;

        /// <summary>
        /// Adds types from additional assemblies to the drop-down list.
        /// By default, only types that can be accessed directly by the class are shown in the list.
        /// </summary>
        [PublicAPI] public string[] IncludeAdditionalAssemblies;

        /// <summary>
        /// Shows all the public types that can be found in the project. By default, only the types that can be
        /// referenced directly by the declaring class are included in the dropdown.
        /// To show internal types too, use <see cref="AllowInternal"/>.
        /// The default value of this parameter can be changed in Project Settings/Packages/Type References.
        /// </summary>
        [PublicAPI] public bool ShowAllTypes;

        /// <summary>
        /// Gets or sets the height of the dropdown. If not set, the height is dynamic (min 100, max 600 pixels).
        /// If set outside the height limits, the height will be clamped.
        /// </summary>
        [PublicAPI] public int DropdownHeight;

        /// <summary>
        /// If the dropdown renders a tree-view, then setting this to <see langword="true"/> will ensure everything
        /// is expanded by default.
        /// </summary>
        [PublicAPI] public bool ExpandAllFolders;

        /// <summary>
        /// Makes the field show the short name of the selected type instead of the full one.
        /// <see langword="false"/> by default.
        /// </summary>
        [PublicAPI] public bool ShortName;

        /// <summary>
        /// If enabled, shows only types that can be serialized by Unity. Defaults to <see langword="false"/>.
        /// </summary>
        [PublicAPI] public bool SerializableOnly;

        /// <summary>
        /// If enabled, includes internal types in the drop-down. By default, only public ones are shown.
        /// </summary>
        [PublicAPI] public bool AllowInternal;

        /// <summary>
        /// Determines whether the specified <see cref="Type"/> matches requirements set in the attribute.
        /// </summary>
        /// <param name="type">Type to test.</param>
        /// <returns>
        /// A <see cref="bool"/> value indicating if the type specified by <paramref name="type"/>
        /// matches the requirements and should thus be selectable.
        /// </returns>
        internal virtual bool MatchesRequirements(Type type)
        {
            bool passesExcludedFilter = ! ExcludeTypes?.Contains(type) ?? true;
            bool passesSerializableFilter = ! SerializableOnly || type.IsUnitySerializable();
            return passesExcludedFilter && passesSerializableFilter;
        }
    }
}