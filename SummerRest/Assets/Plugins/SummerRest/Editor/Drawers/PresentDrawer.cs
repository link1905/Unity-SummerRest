using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(Present<>))]
    internal class PresentDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var main = new VisualElement();
            var hasValue = property.FindPropertyRelative("hasValue");
            var noneLabel = new Label("None");
            var data = new PropertyField(property.FindPropertyRelative("value"));
            main.Add(noneLabel);
            main.Add(data);
            main.CallThenTrackPropertyValue(hasValue, e =>
            {
                noneLabel.Show(!e.boolValue);
                data.Show(e.boolValue);
            });
            return main;
        }
    }
}