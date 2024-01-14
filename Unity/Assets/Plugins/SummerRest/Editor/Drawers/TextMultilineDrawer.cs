using SummerRest.Editor.Attributes;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(TextMultilineAttribute))]
    internal class TextMultilineDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var label = new TextField
            {
                label = property.displayName,
                multiline = true,
            };
            label.style.flexDirection = FlexDirection.Column;
            var att = (TextMultilineAttribute)attribute;
            label.style.flexGrow = label.style.flexShrink = 0f;
            label.style.minHeight = att.MinHeight * EditorGUIUtility.singleLineHeight;
            label.BindProperty(property);
            return label;
        }
    }
}