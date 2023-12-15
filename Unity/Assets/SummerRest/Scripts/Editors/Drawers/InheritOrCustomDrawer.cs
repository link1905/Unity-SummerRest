using SummerRest.Scripts.Attributes;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Scripts.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(InheritOrCustomAttribute))]
    internal class InheritOrCustomDrawer : PropertyDrawer
    {
        private enum Choice
        {
            Inherit, Custom
        }
        private Choice CurrentChoice(SerializedProperty serializedProperty,
            out SerializedProperty inheritBakingProp)
        {
            inheritBakingProp = serializedProperty.FindPropertyRelative($"{serializedProperty.name}BakingInherit");
            var idx = inheritBakingProp.boolValue ? Choice.Inherit : Choice.Custom;
            return idx;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUILayout.LabelField(label);
            var idx = CurrentChoice(property, out var inheritBakingProp);
            idx = (Choice)EditorGUILayout.EnumPopup(idx);
            switch (idx)
            {
                case Choice.Inherit:
                    inheritBakingProp.boolValue = false;
                    break;
                case Choice.Custom:
                    inheritBakingProp.boolValue = true;
                    EditorGUILayout.PropertyField(property, GUIContent.none);
                    break;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var choice = CurrentChoice(property, out _);
            var addHeight = EditorGUIUtility.singleLineHeight;
            if (choice == Choice.Custom)
                addHeight += EditorGUI.GetPropertyHeight(property, true);
            return base.GetPropertyHeight(property, label) + addHeight;
        }
    }
}