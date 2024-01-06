using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Utilities
{
    public static class UIToolkitExtensions
    {
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

        public static void BindOrDisable<T>(this T field, SerializedProperty property) where T : VisualElement, IBindable
        {
            if (property is null)
                field.style.display = DisplayStyle.None;
            else
                field.BindProperty(property);
        }
        
        // public static void BindOrDisable<T>(this T field, SerializedProperty property, string relativeName) where T : VisualElement, IBindable
        // {
        //     field.BindOrDisable(property.FindPropertyRelative(relativeName));
        // }

        public static void BindPropertyNoLabel(this PropertyField field, SerializedProperty property)
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
        
        public static void RemoveLabel(this PropertyField field)
        {
            field.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                field.Q<Label>().style.display = DisplayStyle.None;
            });
        }
    }
}