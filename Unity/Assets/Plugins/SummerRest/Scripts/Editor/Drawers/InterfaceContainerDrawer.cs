using SummerRest.Editor.DataStructures;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(InterfaceContainer<>), true)]
    internal class InterfaceContainerDrawer : UIToolkitDrawer
    {
        public override string RelativeFromTemplateAssetPath => "Properties/interface-container.uxml";
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = Tree;

            var valueProp = property.FindPropertyRelative("value");
            var valuePropField = tree.Q<PropertyField>("value");
            var typePropField = tree.Q<PropertyField>("type");
            typePropField.RegisterValueChangeCallback(e =>
            {
                var newProp = e.changedProperty;
                if (newProp is null)
                    return;
                RebindValuePropField(valuePropField, valueProp);
            });
            return tree;
        }

        private void RebindValuePropField(PropertyField valuePropField, SerializedProperty property)
        {
            valuePropField.Unbind();
            property.serializedObject.Update();
            valuePropField.BindProperty(property);
        }
    }
}