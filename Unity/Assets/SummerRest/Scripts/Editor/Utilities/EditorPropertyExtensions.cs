using System.Reflection;
using SummerRest.Utilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editor.Utilities
{
    public static class EditorPropertyExtensions
    {
        public static void CallMethodOnSerializedObject(this SerializedProperty serializedProperty, string methodName)
        {
            // Use reflection to get the underlying object
            object targetObject = serializedProperty.serializedObject.targetObject;
            // Ensure the object is not null
            if (targetObject != null)
            {
                // Use reflection to find and invoke the method
                var targetType = targetObject.GetType();
                Debug.Log(targetType);
                var methodInfo = targetType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                Debug.Log(methodInfo);
                if (methodInfo != null)
                {
                    methodInfo.Invoke(targetObject, null);
                }
            }
        }
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