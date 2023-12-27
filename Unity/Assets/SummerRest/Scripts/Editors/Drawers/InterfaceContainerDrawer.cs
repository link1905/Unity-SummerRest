using SummerRest.Attributes;
using SummerRest.DataStructures;
using SummerRest.DataStructures.Containers;
using SummerRest.Scripts.Utilities;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace SummerRest.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(InterfaceContainer<>), true)]
    internal class InterfaceContainerDrawer : PropertyDrawer
    {
        private void GetProps(SerializedProperty property,
            out SerializedProperty typeRefProp, out SerializedProperty valueProp)
        {
            typeRefProp = property.FindPropertyRelative("typeReference");
            valueProp = property.FindPropertyRelative("value");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            GetProps(property, out var typeRefProp, out var valueProp);
            using var scope = SummerRestEditorUtilities.LayoutOptions.EditorGUIDrawHorizontalLayout.Create(position);
            scope.PropertyField(typeRefProp);
            EditorGUI.BeginChangeCheck();
            SummerRestEditorUtilities.Drawers.DrawGenericField(scope.NextPosition, valueProp);
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                property.FindPropertyRelative("valueChanged").boolValue = true;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            GetProps(property, out _, out var valueProp);
            return SummerRestEditorUtilities.Sizes.SingleLineHeight + SummerRestEditorUtilities.Sizes.GetGenericPropertyHeight(valueProp);
        }
    }
}