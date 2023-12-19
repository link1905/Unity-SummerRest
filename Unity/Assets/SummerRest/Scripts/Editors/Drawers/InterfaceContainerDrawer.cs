using SummerRest.Attributes;
using SummerRest.DataStructures;
using SummerRest.Editors.Utilities;
using SummerRest.Utilities;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(SerializedGenericFieldAttribute), true)]
    internal class InterfaceContainerDrawer : PropertyDrawer
    {
        private void GetProps(SerializedProperty property,
            out SerializedProperty typeRefProp, out SerializedProperty valueProp)
        {
            var valueContainerProp =
                property.FindSiblingPropertyRelative($"{property.name.FromFieldToUnityFieldName()}Container");
            typeRefProp = valueContainerProp.FindPropertyRelative("typeRef");
            valueProp = valueContainerProp.FindBackingPropertyRelative(nameof(InterfaceContainer<object>.Value));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            GetProps(property, out var typeRefProp, out var valueProp);
            using var scope = EditorCustomUtilities.EditorGUIDrawHorizontalLayout.Create(position);
            scope.LabelLeftField(label);
            scope.PropertyField(typeRefProp);
            EditorGUI.BeginChangeCheck();
            EditorCustomUtilities.DrawGenericField(scope.NextPosition, valueProp);
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
            return EditorCustomUtilities.Heights.SingleLineHeight + EditorCustomUtilities.Heights.GetGenericPropertyHeight(valueProp);
        }
    }
}