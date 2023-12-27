using SummerRest.Models;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class ResponseElement : VisualElement
    {
        private TextField _statusCodeElement;
        public new class UxmlFactory : UxmlFactory<ResponseElement, UxmlTraits>
        {
        }
        public void Init(SerializedProperty serializedProperty)
        {
            if (serializedProperty is null)
            {
                style.Show(false);
                return;
            }
            style.Show(true);
            var statusCodeProp = serializedProperty.FindPropertyRelative("statusCode");
            _statusCodeElement = this.Q<TextField>("status-code");
            _statusCodeElement.Unbind();
            _statusCodeElement.TrackPropertyValue(statusCodeProp, SetStatusCodeValue);
            this.Q<PropertyField>("headers").BindProperty(serializedProperty.FindPropertyRelative("headers"));
            this.Q<TextField>("body").BindProperty(serializedProperty.FindPropertyRelative("body"));
            SetStatusCodeValue(statusCodeProp);
        }

        private void SetStatusCodeValue(SerializedProperty p)
        {
            _statusCodeElement.value = $"{p.enumValueFlag} ({p.enumDisplayNames[p.enumValueIndex]})";
        }
        public ResponseElement()
        {
        }
    }
}