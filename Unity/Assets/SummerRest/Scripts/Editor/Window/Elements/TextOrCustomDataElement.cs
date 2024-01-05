using SummerRest.Editor.Utilities;
using SummerRest.Scripts.Utilities.DataStructures;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    public class TextOrCustomDataElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TextOrCustomDataElement, UxmlTraits>
        {
        }
        public TextOrCustomDataElement()
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
            this.Show(true);
            var typeElement = this.Q<EnumField>("type");
            var typeProp = serializedProperty.FindPropertyRelative("type");
            typeElement.BindProperty(typeProp);
            typeElement.RegisterValueChangedCallback(e =>
            {
                var newVal = e.newValue;
                if (newVal is null)
                    return;
                BindValueElement((TextOrCustomDataType)newVal);
            });
            _textValueElement = this.Q<TextField>("value");
            _genericValueElement = this.Q<PropertyField>("value");
            _textValueElement.BindProperty(serializedProperty.FindPropertyRelative("text"));
            _genericValueElement.BindPropertyNoLabel(serializedProperty.FindPropertyRelative("bodyContainer"));
            BindValueElement((TextOrCustomDataType)typeProp.enumValueIndex);
        }
        private void BindValueElement(TextOrCustomDataType value)
        {
            var showPlain = value == TextOrCustomDataType.PlainText;
            _textValueElement.Show(showPlain);
            _genericValueElement.Show(!showPlain);
        }
    }
}