using SummerRest.Scripts.DataStructures.Primitives;
using SummerRest.Scripts.Editors.Utilities;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Scripts.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(Primitive<>), true)]
    internal class PrimitiveFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var valueProp = property
                .FindBackingPropertyRelative(nameof(Primitive<object>.Value));
            using (var _ = EditorCustomUtilities.DoHorizontalLayout())
            {
                EditorGUILayout.LabelField(label, EditorCustomUtilities.LayoutOptions.Width(label));
                EditorGUILayout.PropertyField(valueProp, GUIContent.none, EditorCustomUtilities.LayoutOptions.ExpandWidth());
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 9f;
        }
    }
}