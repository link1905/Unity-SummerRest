using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(InterfaceContainer<>), true)]
    internal class InterfaceContainerDrawer : UIToolkitDrawer
    {
        protected override string RelativeFromTemplateAssetPath => "Properties/interface-container.uxml";
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = Tree;

            var valueProp = property.FindPropertyRelative("value");
            var valuePropField = tree.Q<PropertyField>("value");
            var typePropField = tree.Q<PropertyField>("type");
            typePropField.TrackPropertyValue(property, _ =>
            {
                RebindValuePropField(valuePropField, valueProp);
            });
            RebindValuePropField(valuePropField, valueProp);
            return tree;
        }

        /// <summary>
        /// Bind to the correct serializedObject because <see cref="InterfaceContainer{T}.Value"/> can be changed based on current <see cref="InterfaceContainer{T}.Type"/>
        /// </summary>
        /// <param name="valuePropField"></param>
        /// <param name="property"></param>
        private void RebindValuePropField(PropertyField valuePropField, SerializedProperty property)
        {
            valuePropField.Unbind();
            property.serializedObject.Update();
            if (property.GetReference() is not null)
                valuePropField.BindProperty(property);
            valuePropField.MarkDirtyRepaint();
        }
    }
}