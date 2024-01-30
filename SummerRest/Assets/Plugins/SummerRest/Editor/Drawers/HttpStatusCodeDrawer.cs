using System.Net;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(HttpStatusCode))]
    internal class HttpStatusCodeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var label = new TextField
            {
                isReadOnly = true,
                label = property.displayName,
            };
            label.CallThenTrackPropertyValue(property, s =>
            {
                var value = (HttpStatusCode)s.enumValueFlag;
                label.SetValueWithoutNotify($"{s.enumValueFlag} ({value})");
            });
            return label;
        }
    }
}