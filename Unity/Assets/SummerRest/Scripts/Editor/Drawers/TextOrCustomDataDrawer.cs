using SummerRest.Editor.Utilities;
using SummerRest.Scripts.Utilities.DataStructures;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(TextOrCustomData<,>), true)]
    internal class TextOrCustomDataDrawer : UIToolkitDrawer
    {
        public override string AssetPath => "Assets/SummerRest/Editors/Templates/Properties/text-or-custom.uxml";
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = Tree;
            var typeElement = tree.Q<EnumField>("type");
            var typeProp = property.FindPropertyRelative("type");
            var textValueElement = tree.Q<TextField>("value");
            var genericValueElement = tree.Q<PropertyField>("value");
            typeElement.RegisterValueChangedCallback(e =>
            {
                var newVal = e.newValue;
                if (newVal is null)
                    return;
                BindValueElement((TextOrCustomDataType)newVal, textValueElement, genericValueElement);
            });
            BindValueElement((TextOrCustomDataType)typeProp.enumValueIndex, textValueElement, genericValueElement);
            return tree;
        }

        private void BindValueElement(TextOrCustomDataType value, TextField textValueElement, PropertyField genericValueElement)
        {
            var showPlain = value == TextOrCustomDataType.PlainText;
            textValueElement.Show(showPlain);
            genericValueElement.Show(!showPlain);
        }

    }
}