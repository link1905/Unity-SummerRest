using SummerRest.Scripts.Utilities.DataStructures;
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
            var hasValue = property.FindPropertyRelative("hasValue");
            if (!hasValue.boolValue)
                return new Label("None");
            return new PropertyField(property.FindPropertyRelative("value"));                
        }
    }
}