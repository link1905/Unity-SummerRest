using System.Collections.Generic;
using System.Reflection;
using SummerRest.Editor.Utilities;
using SummerRest.Scripts.Utilities.Attributes;
using SummerRest.Scripts.Utilities.DataStructures;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(InheritOrCustomContainer<>))]
    internal class InheritOrCustomDrawer : PropertyDrawer
    {
        private const string AssetPath = "Assets/SummerRest/Editors/Templates/Properties/inherit-or-custom.uxml";
        private VisualTreeAsset _treeAsset;
        private InheritOrCustomAttribute _att;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _treeAsset ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetPath);
            _att ??= fieldInfo.GetCustomAttribute(typeof(InheritOrCustomAttribute)) as InheritOrCustomAttribute;
            if (_att is null)
            {
                return new Label(
                    $"Must use {nameof(InheritOrCustomContainer<object>)} with an attribute {nameof(InheritOrCustomAttribute)}");
            }
            var tree = _treeAsset.Instantiate();
            var nameElement = tree.Q<Label>(name: "prop-name");
            nameElement.text = property.displayName;
                        
            var valueElement = tree.Q<PropertyField>(name: "prop");
            valueElement.BindProperty(property.FindPropertyRelative("value"));
            
            var cacheElement = tree.Q<PropertyField>(name: "cache");
            cacheElement.SetEnabled(false);
            //cacheElement.BindProperty(property.FindSiblingBackingPropertyRelative(_att.CachePropName));
            
            var choiceElement = tree.Q<EnumField>(name: "prop-choice");
            var choiceProp = property.FindPropertyRelative("inherit");
            choiceElement.BindProperty(choiceProp);
            choiceElement.RegisterValueChangedCallback(c =>
            {
                if (c.newValue is null)
                    return;
                ShowProp(valueElement, cacheElement, property, choiceProp, (InheritChoice)c.newValue);
            });
            ShowProp(valueElement, cacheElement, property, choiceProp, (InheritChoice)choiceProp.enumValueFlag);
            return tree;
        }

        private InheritChoice? ShouldMoveBackToDefault(SerializedProperty mainProp, InheritChoice selection)
        {
            var parentField = mainProp.FindSiblingBackingPropertyRelative("Parent");
            if (selection is InheritChoice.Inherit or InheritChoice.AppendToParent && parentField?.GetReference() is null)
                return _att.DefaultWhenNoParent;
            var allow = _att.Allow;
            var overlap = allow & selection;
            if (overlap == 0) // Does not overlap
                return _att.Default;
            return null;
        }

        private static readonly Dictionary<InheritChoice, (bool showValue, bool showCache)> ShowState = new()
        {
            { InheritChoice.None, (false, false)},
            { InheritChoice.Inherit, (false, true)},
            { InheritChoice.AppendToParent, (true, true)},
            { InheritChoice.Custom, (true, false)},
        };
        private void ShowProp(VisualElement valueElement, VisualElement cacheElement, SerializedProperty mainProp, SerializedProperty choiceProp, InheritChoice selection)
        {
            var backToDefault = ShouldMoveBackToDefault(mainProp, selection);
            if (backToDefault != null)
            {
                choiceProp.enumValueFlag = (int)backToDefault;
                choiceProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
            if (!ShowState.TryGetValue(selection, out var show)) 
                return;
            valueElement.Show(show.showValue);
            cacheElement.Show(show.showCache);
        }
    }
}