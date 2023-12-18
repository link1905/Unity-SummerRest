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

            EditorCustomUtilities.DrawSequenceHorizontally(position, 
                new EditorCustomUtilities.Section(label.RawWidth(),r => EditorGUI.LabelField(r, label)), 
                new EditorCustomUtilities.Section(r =>
                {
                    var valueProp = property
                        .FindBackingPropertyRelative(nameof(Primitive<object>.Value));
                    EditorGUI.PropertyField(r, valueProp, GUIContent.none);
                }));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorCustomUtilities.Heights.SingleLineHeight;
        }
    }
}