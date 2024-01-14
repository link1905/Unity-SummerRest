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
        public static NativeArray<byte> GetArrayValue(this SerializedProperty serializedProperty)
        {
            var nativeBytes = new NativeArray<byte>(serializedProperty.arraySize, Allocator.Temp);
            for (var i = 0; i < serializedProperty.arraySize; i++)
                nativeBytes[i] = (byte)serializedProperty.GetArrayElementAtIndex(i).intValue;
            return nativeBytes;
        }
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
        public static void FitLabel<T>(this BaseField<T> baseField)
        {
            baseField.labelElement.style.minWidth = StyleKeyword.Auto;
        }
        public static void FitLabel<T>(params BaseField<T>[] baseField)
        {
            foreach (var b in baseField)
                b.FitLabel();
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

        public static void InsertAll(this VisualElement visualElement, int startIndex, params VisualElement[] elements)
        {
            foreach (var element in elements)
                visualElement.Insert(startIndex++, element);
        }
        public static void BindWithCallback<TField, TCallbackValue>(this TField field, SerializedObject obj, Action<TCallbackValue> callback) 
            where TField : IBindable, INotifyValueChanged<TCallbackValue>
        {
            field.BindProperty(obj);
            field.RegisterValueChangedCallback(e =>
            {
                var changed = e.newValue;
                if (changed is null)
                    return;
                callback?.Invoke(changed);
            });
        }
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