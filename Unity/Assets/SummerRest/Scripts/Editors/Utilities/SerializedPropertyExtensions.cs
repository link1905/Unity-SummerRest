using System.Collections.Generic;
using UnityEditor;

namespace SummerRest.Scripts.Editors.Utilities
{
    public static class SerializedPropertyExtensions
    {
        private static readonly Dictionary<string, string> CacheBackingName = new();
        public static SerializedProperty FindBackingPropertyRelative(
            this SerializedProperty self, string name)
        {
            if (!CacheBackingName.TryGetValue(name, out var backingName))
            {
                backingName = $"<{name}>k__BackingField";
                CacheBackingName.Add(name, backingName);
            }
            return self.FindPropertyRelative(backingName);
        }
    }
}