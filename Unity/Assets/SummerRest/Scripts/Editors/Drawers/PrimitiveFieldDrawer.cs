using SummerRest.DataStructures.Primitives;
using SummerRest.Editors.Utilities;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(Primitive<>), true)]
    internal class PrimitiveFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using var scope = EditorCustomUtilities.EditorGUIDrawHorizontalLayout.Create(position);
            scope.LabelLeftField(label);
            var valueProp = property
                .FindBackingPropertyRelative(nameof(Primitive<object>.Value));
            scope.PropertyField(valueProp, label: GUIContent.none);
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorCustomUtilities.Heights.SingleLineHeight;
        }
    }
}