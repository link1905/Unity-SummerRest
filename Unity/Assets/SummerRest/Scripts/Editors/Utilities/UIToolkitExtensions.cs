using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Utilities
{
    public static class UIToolkitExtensions
    {
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
    }
}