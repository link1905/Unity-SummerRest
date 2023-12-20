using SummerRest.Attributes;
using SummerRest.DataStructures;
using SummerRest.Editors.Utilities;
using SummerRest.Utilities;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(SerializedGenericFieldAttribute), true)]
    internal class SerializedGenericFieldAttributeDrawer : PropertyDrawer
    {
        private void GetProps(SerializedProperty property, out SerializedProperty valueContainerProp,
            out SerializedProperty typeRefProp, out SerializedProperty valueProp)
        {
            valueContainerProp =
                property.FindSiblingPropertyRelative($"{property.name.FromFieldToUnityFieldName()}Container");
            typeRefProp = valueContainerProp.FindPropertyRelative("typeRef");
            valueProp = valueContainerProp.FindBackingPropertyRelative(nameof(InterfaceContainer<object>.Value));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            GetProps(property, out var valueContainerProp, out var typeRefProp, out var valueProp);
            using var scope = SummerRestEditorUtilities.LayoutOptions.EditorGUIDrawHorizontalLayout.Create(position);
            scope.LabelLeftField(label);
            scope.PropertyField(typeRefProp);
            EditorGUI.BeginChangeCheck();
            SummerRestEditorUtilities.Drawers.DrawGenericField(scope.NextPosition, valueProp);
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                valueContainerProp.FindPropertyRelative("valueChanged").boolValue = true;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            GetProps(property, out _, out _, out var valueProp);
            return SummerRestEditorUtilities.Sizes.SingleLineHeight + SummerRestEditorUtilities.Sizes.GetGenericPropertyHeight(valueProp);
        }
    }
}