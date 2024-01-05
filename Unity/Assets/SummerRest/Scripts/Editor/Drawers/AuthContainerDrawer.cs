using SummerRest.Editor.Models;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(AuthContainer))]
    internal class AuthContainerDrawer : TextOrCustomDataDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = base.CreatePropertyGUI(property);
            var textField = new TextField("Key");
            textField.BindProperty(property.FindPropertyRelative("key"));
            tree.Insert(0, textField);
            return tree;
        }
    }
}