using SummerRest.Editor.Attributes;
using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ReadonlyTextAttribute))]
    internal class ReadonlyTextDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var field = new TextField
            {
                label = property.displayName,
                multiline = false,
                isReadOnly = true,
                bindingPath = property.name
            };
            return field;
        }
    }
}