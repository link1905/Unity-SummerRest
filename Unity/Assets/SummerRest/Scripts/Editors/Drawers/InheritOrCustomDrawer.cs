using SummerRest.Attributes;
using SummerRest.Editors.Utilities;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editors.Drawers
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
            inheritBakingProp = serializedProperty.FindSiblingPropertyRelative($"{serializedProperty.name}InheritCheck");
            var parentField = serializedProperty.FindSiblingBackingPropertyRelative("Parent");
            if (parentField.managedReferenceValue is null)
                return Choice.Custom;
            var idx = inheritBakingProp.boolValue ? Choice.Inherit : Choice.Custom;
            return idx;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            using var scope = SummerRestEditorUtilities.LayoutOptions.EditorGUIDrawHorizontalLayout.Create(position);
            scope.LabelLeftField(label);
            var idx = CurrentChoice(property, out var inheritBakingProp);
            idx = scope.EnumPopup(idx);
            switch (idx)
            {
                case Choice.Inherit:
                    inheritBakingProp.boolValue = true;
                    break;
                case Choice.Custom:
                    inheritBakingProp.boolValue = false;
                    EditorGUI.PropertyField(scope.NextPosition, property, GUIContent.none, true);
                    break;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var choice = CurrentChoice(property, out _);
            var addHeight = SummerRestEditorUtilities.Sizes.SingleLineHeight;
            if (choice == Choice.Custom)
                addHeight += SummerRestEditorUtilities.Sizes.GetPropertyHeight(property, label);
            return addHeight;
        }
    }
}