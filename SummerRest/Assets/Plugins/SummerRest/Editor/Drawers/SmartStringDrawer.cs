using SummerRest.Editor.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SmartString))]
    internal class SmartStringDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var mainContainer = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row }
            };
            var label = new Label
            {
                style = { flexGrow = 0f, flexShrink = 0f, unityTextAlign = TextAnchor.MiddleCenter },
                bindingPath = "key"
            };
            var value = new TextField
            {
                style = { flexGrow = 1f },
                bindingPath = "value"
            };
            mainContainer.Add(label);
            mainContainer.Add(value);
            return mainContainer;
        }
    }
}