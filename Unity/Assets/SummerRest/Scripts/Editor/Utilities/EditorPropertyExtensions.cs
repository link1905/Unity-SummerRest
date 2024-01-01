using SummerRest.Scripts.Utilities.Extensions;
using UnityEditor;

namespace SummerRest.Editor.Utilities
{
    public static class EditorPropertyExtensions
    {

        public static object GetReference(this SerializedProperty self)
        {
            switch(self.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    return self.objectReferenceValue;
                case SerializedPropertyType.ManagedReference:
                    return self.managedReferenceValue;
                default:
                    return null;
            }
        }
        public static SerializedProperty FindSiblingBackingPropertyRelative(
            this SerializedProperty self, string name)
        {
            var backingName = $"<{name}>k__BackingField";
            return self.FindSiblingPropertyRelative(backingName);
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