using System;
using System.Linq;
using SummerRest.Scripts.Attributes;
using SummerRest.Scripts.Editors.Utilities;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Scripts.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(DefaultsAttribute))]
    internal class DefaultsDrawer : PropertyDrawer
    {
        private string[] _defaultValues;
        private GUILayoutOption _labelOption;
        private GUILayoutOption _customOption;
        private void Init(SerializedProperty property, GUIContent label)
        {
            if (_defaultValues is null)
            {
                _defaultValues = ((DefaultsAttribute)attribute).Defaults.Prepend("Custom").ToArray();
                _labelOption = EditorCustomUtilities.Width(label);
                _customOption = EditorCustomUtilities.Width(_defaultValues[0]);
            }
        }
        public int GetIdx(SerializedProperty property)
        {
            return Array.IndexOf(_defaultValues, property.stringValue);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property, label);
            EditorGUI.BeginProperty(position, label, property);
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUILayout.HelpBox(
                    $"Can not use {nameof(DefaultsAttribute)} with others types but string",
                    MessageType.Error);
                return;
            }
            var oldIdx = Mathf.Max(0, GetIdx(property));
            var oldVal = property.stringValue;
            using var scope = EditorCustomUtilities.DoHorizontalLayout(
                GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(label, _labelOption);
            var selectIdx = EditorGUILayout.Popup(oldIdx, _defaultValues, 
                oldIdx != 0 ? GUILayout.ExpandWidth(true) : _customOption);
            if (oldIdx != selectIdx)
                property.stringValue = _defaultValues[selectIdx];
            if (selectIdx == 0)
            {                
                //property.stringValue = "";
                EditorGUILayout.PropertyField(property, GUIContent.none, GUILayout.ExpandWidth(true));
            }
            EditorGUI.EndProperty();
        }
    }
}