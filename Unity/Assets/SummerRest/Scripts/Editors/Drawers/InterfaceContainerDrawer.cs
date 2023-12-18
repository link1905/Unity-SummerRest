using SummerRest.Scripts.DataStructures;
using SummerRest.Scripts.Editors.Utilities;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Scripts.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(InterfaceContainer<>))]
    internal class InterfaceContainerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var typeRefProp = property.FindPropertyRelative("typeRef");
            using (EditorCustomUtilities.DoHorizontalLayout(EditorCustomUtilities.LayoutOptions.ExpandWidth()))
            {
                EditorGUILayout.LabelField(label, EditorCustomUtilities.LayoutOptions.Width(label));
                EditorGUILayout.PropertyField(typeRefProp, GUIContent.none, EditorCustomUtilities.LayoutOptions.ExpandWidth());
            }
            EditorGUI.BeginChangeCheck();
            var valueProp = property.FindBackingPropertyRelative("Value");
            valueProp.DrawGenericField();
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                property.FindPropertyRelative("valueChanged").boolValue = true;
            }
            EditorGUI.EndProperty();
        }
    }
}