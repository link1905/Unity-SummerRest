using System.Net;
using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(HttpStatusCode))]
    internal class HttpStatusCodeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var value = (HttpStatusCode)property.enumValueFlag;
            var label = new TextField
            {
                isReadOnly = true,
                label = property.displayName,
            };
            label.SetValueWithoutNotify($"{property.enumValueFlag} ({value})");
            return label;
        }
    }
}