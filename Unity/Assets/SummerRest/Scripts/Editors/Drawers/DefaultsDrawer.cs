using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SummerRest.Attributes;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(DefaultsAttribute))]
    internal class DefaultsDrawer : PropertyDrawer
    {
        private const string AssetPath = "Assets/SummerRest/Editors/Templates/Properties/default_or_custom.uxml";
        private VisualTreeAsset _treeAsset;
        private List<string> _defaultValues;
        private void Init()
        {
            if (_defaultValues is null)
            {
                _defaultValues = ((DefaultsAttribute)attribute).Defaults.Prepend("Custom").ToList();
                _treeAsset ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetPath);
            }
        }
        public int GetIdx(string value)
        {
            var find = _defaultValues.IndexOf(value);
            return Mathf.Max(0, find);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Init();
            var tree = _treeAsset.Instantiate();
            
            var nameElement = tree.Q<Label>(name: "name");
            nameElement.text = property.displayName;
            
            var customElement = tree.Q<TextField>("custom");
            customElement.BindProperty(property);
            
            var valuesElement = tree.Q<DropdownField>(name: "values");
            valuesElement.choices = _defaultValues;
            valuesElement.index = GetIdx(property.stringValue);
            valuesElement.RegisterValueChangedCallback(e =>
            {
                var val = e.newValue;
                if (val is not null)
                    ShowCustomElement(customElement, property, val);
            });
            ShowCustomElement(customElement, property, property.stringValue);
            return tree;
        }
        private void ShowCustomElement(TextField customElement, SerializedProperty property, string newVal)
        {
            property.serializedObject.Update();
            var currentIdx = GetIdx(property.stringValue);
            var newIdx = GetIdx(newVal);
            if (currentIdx != newIdx) //Custom values always return 0
                customElement.value = newVal;
            customElement.Show(newIdx == 0);
        }
    }
}