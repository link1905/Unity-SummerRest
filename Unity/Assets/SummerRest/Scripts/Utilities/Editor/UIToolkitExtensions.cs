using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Scripts.Utilities.Editor
{
    public static class UIToolkitExtensions
    {
        public static void BindChildrenToProperties(this VisualElement visualElement, SerializedObject serializedObject)
        {
            foreach (var child in visualElement.Children())
            {
                if (child is not IBindable bindableElement || string.IsNullOrEmpty(bindableElement.bindingPath))
                    continue;
                bindableElement.BindProperty(serializedObject);
            }
        } 
        public static void UnBindAllChildren(this VisualElement visualElement)
        {
            foreach (var child in visualElement.Children())
            {
                if (child is not BindableElement bindableElement)
                    continue;
                bindableElement.Unbind();
            }
        } 
        public static void Visible(this IStyle style, bool visible)
        {
            style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }
        public static void Show(this IStyle style, bool show)
        {
            style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
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