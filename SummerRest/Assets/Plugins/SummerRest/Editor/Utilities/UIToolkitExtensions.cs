using System;
using Unity.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Utilities
{
    public static class UIToolkitExtensions
    {
        public static void ReplaceBackgroundColor(this IStyle style, Color color)
        {
            var oldStyle = style.backgroundColor;
            oldStyle.value = color;
            style.backgroundColor = oldStyle;
        }
        
        
        
        public static SerializedProperty FindBackingPropertyRelative(this SerializedObject serializedObject, string name)
        {
            return serializedObject.FindProperty($"<{name}>k__BackingField");
        }
        public static object GetReference(this SerializedProperty serializedObject)
        {
            switch (serializedObject.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    return serializedObject.objectReferenceValue;
                case SerializedPropertyType.ManagedReference:
                    return serializedObject.managedReferenceValue;
            }
            return null;
        }
        public static SerializedProperty FindBackingPropertyRelative(this SerializedProperty serializedObject, string name)
        {
            return serializedObject.FindPropertyRelative($"<{name}>k__BackingField");
        }
        public static NativeArray<byte> GetArrayValue(this SerializedProperty serializedProperty)
        {
            var nativeBytes = new NativeArray<byte>(serializedProperty.arraySize, Allocator.Temp);
            for (var i = 0; i < serializedProperty.arraySize; i++)
                nativeBytes[i] = (byte)serializedProperty.GetArrayElementAtIndex(i).intValue;
            return nativeBytes;
        }
        /// <summary>
        /// Find all bindable children of <see cref="visualElement"/> and bind them to <see cref="serializedObject"/>
        /// </summary>
        /// <param name="visualElement"></param>
        /// <param name="serializedObject"></param>
        public static void BindChildrenToProperties(this VisualElement visualElement, SerializedObject serializedObject)
        {
            foreach (var child in visualElement.Children())
            {
                if (child.childCount > 0)
                    BindChildrenToProperties(child, serializedObject);
                if (child is not IBindable bindableElement || string.IsNullOrEmpty(bindableElement.bindingPath))
                    continue;
                var prop = serializedObject.FindProperty(bindableElement.bindingPath);
                if (prop is null)
                {
                    child.Show(false);
                    child.Unbind();
                }
                else
                {
                    child.Show(true);
                    bindableElement.BindProperty(prop);
                }
            }
        } 
        public static void UnBindAllChildren(this VisualElement visualElement)
        {
            foreach (var child in visualElement.Children())
            {
                if (child.childCount > 0)
                    UnBindAllChildren(child);
                if (child is not BindableElement bindableElement)
                    continue;
                bindableElement.Unbind();
            }
        }
        /// <summary>
        /// A <see cref="PropertyField"/> often comes with a label displaying propertyDisplayName <br/>
        /// This method remove that label
        /// </summary>
        /// <param name="field"></param>
        /// <param name="property"></param>
        public static void BindPropertyNoLabel(this PropertyField field, SerializedObject property)
        {
            field.BindProperty(property);
            field.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                var label = field.Q<Label>();
                if (label is null)
                    return;
                label.style.display = DisplayStyle.None;
            });
        }
        public static void Show(this IStyle style, bool show)
        {
            style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
        public static void Show(this VisualElement visualElement, bool show)
        {
            visualElement.style.Show(show);
        }
        public static void SetTextValueWithoutNotify<TField>(this TField field, string val) 
            where TField : INotifyValueChanged<string>
        {
            field.SetValueWithoutNotify(val);
        }
        /// <summary>
        /// Binds an element to a <see cref="SerializedObject"/> and invokes <see cref="callback"/> whenever the property is changed
        /// </summary>
        /// <param name="field"></param>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        /// <typeparam name="TField"></typeparam>
        /// <typeparam name="TCallbackValue"></typeparam>
        public static void BindWithCallback<TField, TCallbackValue>(this TField field, SerializedObject obj, Action<TCallbackValue> callback) 
            where TField : IBindable, INotifyValueChanged<TCallbackValue>
        {
            field.BindProperty(obj);
            // Use this instead of TrackPropertyValue because the method does not working properly (it's currently a known issue)
            field.RegisterValueChangedCallback(e =>
            {
                var changed = e.newValue;
                if (changed is null)
                    return;
                callback?.Invoke(changed);
            });
        }
        
        /// <summary>
        /// Binds an element to a <see cref="SerializedProperty"/> and invokes <see cref="callback"/> whenever the property is changed
        /// </summary>
        /// <param name="field"></param>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        /// <typeparam name="TField"></typeparam>
        /// <typeparam name="TCallbackValue"></typeparam>
        public static void BindWithCallback<TField, TCallbackValue>(this TField field, SerializedProperty property, Action<TCallbackValue> callback) 
            where TField : IBindable, INotifyValueChanged<TCallbackValue>
        {
            field.BindProperty(property);
            field.RegisterValueChangedCallback(e =>
            {
                var changed = e.newValue;
                if (changed is null)
                    return;
                callback?.Invoke(changed);
            });
        }
    }
}