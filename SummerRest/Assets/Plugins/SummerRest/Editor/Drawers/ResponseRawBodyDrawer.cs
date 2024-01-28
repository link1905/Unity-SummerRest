using System;
using System.IO;
using SummerRest.Editor.Attributes;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ResponseRawBodyAttribute))]
    internal class ResponseRawBodyDrawer : TextMultilineDrawer
    {
        private const int MaxLength = 5000;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            var exceedBox = new HelpBox(
                "This body is not complete: Unity does not allow to show a very long text, please save it as a file", HelpBoxMessageType.Info);

            var label = TextField(property);
            label.isReadOnly = true;
            container.CallThenTrackPropertyValue(property, s =>
            {
                var exceed = s.stringValue.Length >= MaxLength;
                exceedBox.Show(exceed);
                label.value = exceed ? s.stringValue[..MaxLength] : s.stringValue;
            });
            container.Add(exceedBox);
            container.Add(label);
            return container;
        }
    }
}