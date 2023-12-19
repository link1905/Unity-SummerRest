using System;
using System.Collections.Generic;
using SummerRest.Utilities;
using UnityEditor;

namespace SummerRest.Editors.Utilities
{
    public static class SerializedPropertyExtensions
    {
        public static SerializedProperty FindBackingPropertyRelative(
            this SerializedProperty self, string name)
        {
            var backingName = $"<{name}>k__BackingField";
            return self.FindPropertyRelative(backingName);
        }
        public static SerializedProperty FindSiblingPropertyRelative(
            this SerializedProperty self, string name)
        {
            var dotIndex = self.propertyPath.LastIndexOf('.');
            var siblingPath = dotIndex == -1 ? name : self.propertyPath.ReplaceFromIndexWith(dotIndex + 1, name);
            return self.serializedObject.FindProperty(siblingPath);
        }
    }
}