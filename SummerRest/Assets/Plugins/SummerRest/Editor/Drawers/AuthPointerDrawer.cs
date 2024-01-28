using System;
using System.Collections.Generic;
using System.Linq;
using SummerRest.Editor.Configurations;
using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    
    [CustomPropertyDrawer(typeof(AuthPointer))]
    internal class AuthPointerDrawer : UIToolkitDrawer
    {
        private static int GetIndex(IList<string> options, string current)
        {
            var idx = options.IndexOf(current);
            return Mathf.Max(idx, 0);
        }

        protected override string RelativeFromTemplateAssetPath => "Properties/auth-pointer.uxml";
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = Tree;
            var authConfigure= SummerRestConfiguration.Instance.AuthContainers;
            var allIds = authConfigure.Select(e => e.AuthKey).ToList();
            var noOption = allIds.Count == 0;
            
            var noOptionHelpBox = tree.Q<HelpBox>("no-option");
            var selectionsDropdown = tree.Q<DropdownField>();
            noOptionHelpBox.Show(noOption);
            selectionsDropdown.Show(!noOption);

            var keyProp = property.FindPropertyRelative("authKey");
            var previewField = tree.Q<PropertyField>("preview");
            // Only modify dropdown and preview section when having at least 1 auth container configured
            if (!noOption)
            {
                //Make sure the the property pointing to an auth container (default is 0)
                selectionsDropdown.choices = allIds;
                selectionsDropdown.RegisterValueChangedCallback(s =>
                {
                    ShowPreview(previewField, s.newValue);
                });
                keyProp.stringValue = allIds[GetIndex(allIds, keyProp.stringValue)];
                keyProp.serializedObject.ApplyModifiedProperties();
            }
            return tree;
        }
        private void ShowPreview(PropertyField previewField, string val)
        {
            var authConfigure= SummerRestConfiguration.Instance;
            var idx = authConfigure.AuthContainers.FindIndex(s => s.AuthKey == val);
            if (idx < 0)
            {
                previewField.Show(false);
                return;
            }
            previewField.Show(true);
            var authSerObj = new SerializedObject(authConfigure);
            var authsArr = authSerObj.FindProperty("authContainers");
            // Bind the auth container to preview it inside an AuthPointer
            previewField.BindProperty(authsArr.GetArrayElementAtIndex(idx));
        }
    }
}