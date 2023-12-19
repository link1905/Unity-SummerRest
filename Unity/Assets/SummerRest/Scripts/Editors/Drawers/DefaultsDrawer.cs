using System;
using System.Linq;
using SummerRest.Attributes;
using SummerRest.Editors.Utilities;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(DefaultsAttribute))]
    internal class DefaultsDrawer : PropertyDrawer
    {
        private string[] _defaultValues;
        private GUILayoutOption _labelOption;
        private float _customWidth;
        private void Init(SerializedProperty property, GUIContent label)
        {
            if (_defaultValues is null)
            {
                _defaultValues = ((DefaultsAttribute)attribute).Defaults.Prepend("Custom").ToArray();
                _labelOption = label.Width();
                _customWidth = _defaultValues[0].RawWidth();
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
                EditorGUI.HelpBox(position,
                    $"Can not use {nameof(DefaultsAttribute)} with others types but string",
                    MessageType.Error);
                return;
            }
            var oldIdx = Mathf.Max(0, GetIdx(property));
            var oldVal = property.stringValue;
            using var scope = EditorCustomUtilities.EditorGUIDrawHorizontalLayout.Create(position);
            scope.LabelLeftField(label);
            var width = oldIdx != 0 ? -1 : _customWidth;
            var selectIdx = scope.PopupField(_defaultValues, oldIdx, width);
            if (oldIdx != selectIdx)
                property.stringValue = _defaultValues[selectIdx];
            if (selectIdx == 0)
            {                
                //property.stringValue = "";
                scope.PropertyField(property);
            }
            EditorGUI.EndProperty();
        }
    }
}