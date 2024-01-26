using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SummerRest.Editor.Attributes;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(InheritOrCustomContainer<>))]
    internal class InheritOrCustomDrawer : UIToolkitDrawer
    {
        protected override string RelativeFromTemplateAssetPath => "Properties/inherit-or-custom.uxml";
        private InheritOrCustomAttribute _att;
        private List<string> _allows;
        private List<string> GetAllows(SerializedProperty serializedProperty, InheritChoice allow)
        {
            var result = new List<string>();
            var parent = serializedProperty.serializedObject.FindBackingPropertyRelative(_att.ParentPropName);
            var parentValue = parent?.objectReferenceValue;
            // Select values overlapping with Allow in the InheritOrCustomAttribute
            foreach (InheritChoice choice in Enum.GetValues(typeof(InheritChoice)))
            {
                var overlap = choice & allow;
                //Inherit or AppendToParent => must have a parent
                if (overlap != 0 && (parentValue is not null || 
                                     (choice != InheritChoice.Inherit && choice != InheritChoice.AppendToParent)))
                    result.Add(choice.ToString());
            }
            return result;
        }
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = Tree;
            _att ??= fieldInfo.GetCustomAttribute(typeof(InheritOrCustomAttribute)) as InheritOrCustomAttribute;
            if (_att is null)
            {
                return new Label(
                    $"Must use {nameof(InheritOrCustomContainer<object>)} with an attribute {nameof(InheritOrCustomAttribute)}");
            }
            _allows ??= GetAllows(property, _att.Allow);
            
            var nameElement = tree.Q<Foldout>(name: "container");
            nameElement.text = property.displayName;
                        
            var valueElement = tree.Q<PropertyField>(name: "prop");
            valueElement.BindProperty(property.FindPropertyRelative("value"));
     
            
            var cacheElement = tree.Q<PropertyField>(name: "cache");
            cacheElement.SetEnabled(false);
            var cacheProp = property.FindPropertyRelative("cache");
            if (cacheProp is not null)
                cacheElement.BindProperty(cacheProp);
            
            var choiceElement = tree.Q<DropdownField>(name: "prop-choice");
            var choiceProp = property.FindPropertyRelative("inherit");
            choiceElement.choices = _allows;
            choiceElement.SetValueWithoutNotify(((InheritChoice)choiceProp.enumValueFlag).ToString());
            choiceElement.RegisterValueChangedCallback(c =>
            {
                if (!Enum.TryParse<InheritChoice>(c.newValue, out var val))
                    return;
                choiceProp.enumValueFlag = (int)val;
                choiceProp.serializedObject.ApplyModifiedProperties();
                ShowProp(valueElement, cacheElement, val);
            });
            ShowProp(valueElement, cacheElement, (InheritChoice)choiceProp.enumValueFlag);
            return tree;
        }
        private static readonly Dictionary<InheritChoice, (bool showValue, bool showCache)> ShowState = new()
        {
            { InheritChoice.None, (false, false)},
            { InheritChoice.Inherit, (false, true)},
            { InheritChoice.AppendToParent, (true, true)},
            { InheritChoice.Custom, (true, false)},
            { InheritChoice.Auto, (false, true)},
        };
        private void ShowProp(VisualElement valueElement, VisualElement cacheElement, InheritChoice selection)
        {
            if (!ShowState.TryGetValue(selection, out var show)) 
                return;
            valueElement.Show(show.showValue);
            cacheElement.Show(show.showCache);
        }

    }
}