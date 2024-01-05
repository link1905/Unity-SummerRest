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
        public override string AssetPath => "Assets/SummerRest/Editors/Templates/Properties/auth-pointer.uxml";
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = Tree;
            var authConfigure= SummerRestConfigurations.Instance.AuthenticateConfigurations;
            if (authConfigure is null)
                throw new Exception($"There is no {nameof(AuthenticateConfigurations)} in the project");
            var allIds = authConfigure.AuthContainers.Select(e => e.Key).ToList();
            var noOption = allIds.Count == 0;
            
            var noOptionHelpBox = tree.Q<HelpBox>("no-option");
            var selectionsDropdown = tree.Q<DropdownField>();
            noOptionHelpBox.Show(noOption);
            selectionsDropdown.Show(!noOption);

            var keyProp = property.FindPropertyRelative("authKey");
            var previewField = tree.Q<PropertyField>("preview");
            // previewField.SetEnabled(false);
            // If no key ids are matched => get the first one 
            if (!noOption)
            {
                keyProp.stringValue = allIds[GetIndex(allIds, keyProp.stringValue)];
                selectionsDropdown.choices = allIds;
                selectionsDropdown.BindWithCallback<DropdownField, string>(keyProp, s =>
                {
                    ShowPreview(previewField, s);
                });
            }
            return tree;
        }
        private void ShowPreview(PropertyField previewField, string val)
        {
            var authConfigure= SummerRestConfigurations.Instance.AuthenticateConfigurations;
            var idx = Array.FindIndex(authConfigure.AuthContainers, s => s.Key == val); 
            var authSerObj = new SerializedObject(authConfigure);
            var authsArr = authSerObj.FindProperty("auths");
            // authConfigure.AuthContainers.FirstOrDefault(e => e.Key == val)
            previewField.BindProperty(authsArr.GetArrayElementAtIndex(idx));
        }
    }
}