using SummerRest.Models;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class RequestBodyElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<RequestBodyElement, UxmlTraits>
        {
        }
        public RequestBodyElement()
        {
        }

        private TextField _textValueElement;
        private PropertyField _genericValueElement;
        public void Init(SerializedProperty serializedProperty)
        {
            if (serializedProperty is null)
            {
                this.Show(false);
                return;
            }

            this.Q<Label>("name").text = serializedProperty.displayName;
            var typeElement = this.Q<EnumField>();
            var typeProp = serializedProperty.FindPropertyRelative("type");
            typeElement.BindProperty(typeProp);
            typeElement.RegisterValueChangedCallback(e =>
            {
                var newVal = e.newValue;
                if (newVal is null)
                    return;
                BindValueElement((RequestBodyType)newVal);
            });
            _textValueElement = this.Q<TextField>("value");
            _genericValueElement = this.Q<PropertyField>("value");
            _textValueElement.BindProperty(serializedProperty.FindPropertyRelative("text"));
            _genericValueElement.BindPropertyNoLabel(serializedProperty.FindPropertyRelative("bodyContainer"));
            BindValueElement((RequestBodyType)typeProp.enumValueIndex);
        }
        private void BindValueElement(RequestBodyType value)
        {
            var showPlain = value == RequestBodyType.PlainText;
            _textValueElement.Show(showPlain);
            _genericValueElement.Show(!showPlain);
        }
    }
}