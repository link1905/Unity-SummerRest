using System.Reflection;
using SummerRest.Attributes;
using SummerRest.DataStructures;
using SummerRest.DataStructures.Containers;
using SummerRest.DataStructures.Enums;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(InheritOrCustomContainer<>))]
    internal class InheritOrCustomDrawer : PropertyDrawer
    {

        private const string AssetPath = "Assets/SummerRest/Editors/Templates/Properties/inherit_or_custom_UXML.uxml";
        private VisualTreeAsset _treeAsset;
        private InheritOrCustomAttribute _att;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _treeAsset ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetPath);
            _att ??= fieldInfo.GetCustomAttribute(typeof(InheritOrCustomAttribute)) as InheritOrCustomAttribute;
            if (_att is null)
            {
                return new Label(
                    $"Must you {nameof(InheritOrCustomContainer<object>)} with an attribute {nameof(InheritOrCustomAttribute)}");
            }
            var tree = _treeAsset.Instantiate();
            var nameElement = tree.Q<Label>(name: "prop-name");
            nameElement.text = property.displayName;
                        
            var valueElement = tree.Q<PropertyField>(name: "prop");
            valueElement.BindPropertyNoLabel(property.FindPropertyRelative("value"));
            
            var choiceElement = tree.Q<EnumField>(name: "prop-choice");
            var choiceProp = property.FindPropertyRelative("inherit");
            choiceElement.BindProperty(choiceProp);
            choiceElement.RegisterValueChangedCallback(c =>
            {
                if (c.newValue is null)
                    return;
                ShowProp(valueElement, property, choiceProp, (InheritChoice)c.newValue);
            });
            ShowProp(valueElement, property, choiceProp, (InheritChoice)choiceProp.enumValueFlag);
            return tree;
        }

        private InheritChoice? ShouldMoveBackToDefault(SerializedProperty mainProp, InheritChoice selection)
        {
            var parentField = mainProp.FindSiblingPropertyRelative("parent");
            if (selection is InheritChoice.Inherit or InheritChoice.AppendToParent && parentField?.GetReference() is null)
                return _att.DefaultWhenNoParent;
            var allow = _att.Allow;
            var overlap = allow & selection;
            if (overlap == 0) // Does not overlap
                return _att.Default;
            return null;
        }
        
        private void ShowProp(VisualElement valueElement, SerializedProperty mainProp, SerializedProperty choiceProp, InheritChoice selection)
        {
            var backToDefault = ShouldMoveBackToDefault(mainProp, selection);
            if (backToDefault != null)
            {
                choiceProp.enumValueFlag = (int)backToDefault;
                choiceProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                var validChangeProp = mainProp.FindPropertyRelative("validChange");
            }
            switch (selection)
            {
                case InheritChoice.None or InheritChoice.Inherit:
                    valueElement.style.display = DisplayStyle.None;
                    break;
                case InheritChoice.Custom or InheritChoice.AppendToParent:
                    valueElement.style.display = DisplayStyle.Flex;
                    break;
                default:
                    return;
            }
            
        }
    }
}