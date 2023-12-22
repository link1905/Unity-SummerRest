using System;
using System.Collections.Generic;
using System.Reflection;
using SolidUtilities.UnityEditorInternals;
using SummerRest.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Utilities
{
    public static class SerializedPropertyExtensions
    {
        public static void BindOrDisable<T>(this T field, SerializedProperty property) where T : VisualElement, IBindable
        {
            if (property is null)
                field.style.display = DisplayStyle.None;
            else
                field.BindProperty(property);
        }
        
        public static void BindOrDisable<T>(this T field, SerializedProperty property, string relativeName) where T : VisualElement, IBindable
        {
            field.BindOrDisable(property.FindPropertyRelative(relativeName));
        }

        public static void BindPropertyNoLabel(this PropertyField field, SerializedProperty property)
        {
            field.BindProperty(property);
            field.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                field.Q<Label>().style.display = DisplayStyle.None;
            });
        }
        
        public static void RemoveLabel(this PropertyField field)
        {
            field.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                field.Q<Label>().style.display = DisplayStyle.None;
            });
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
        public static T GetNonSerializedPropertyValue<T>(this SerializedProperty serializedProperty, string fieldName)
        {
            // Ensure that the property is valid and its serialized object is not null
            if (serializedProperty is { serializedObject: not null })
            {
                // Get the target object from the serialized object
                var type = serializedProperty.GetFieldInfoAndType().Type;
                var prop = type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public);
                if (prop != null)
                {
                    var target = serializedProperty.serializedObject.targetObject;
                    return (T)prop.GetValue(target);
                }
            }
            return default;
        }
        public static SerializedProperty FindParentProperty(this SerializedProperty self)
        {
            var dotIndex = self.propertyPath.LastIndexOf('.');
            var siblingPath = dotIndex == -1 ? "" : self.propertyPath[..(dotIndex + 1)];
            return self.serializedObject.FindProperty(siblingPath);
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