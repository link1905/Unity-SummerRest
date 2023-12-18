using SummerRest.DataStructures;
using SummerRest.Editors.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SummerRest.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(OptionsArray<>))]
    internal class OptionsArrayDrawer : PropertyDrawer
    {
        private ReorderableList _reorderableList;
        private ReorderableList ReorderableList(SerializedProperty property, GUIContent label)
        {
            return _reorderableList ??= OnEnable(property, label);
        }

        
        private void DrawOptionElement(SerializedProperty indexProp, SerializedProperty valuesProp, Rect rect, int idx)
        {
            var element = valuesProp.GetArrayElementAtIndex(idx);
            EditorCustomUtilities.DrawSequenceHorizontally(
                rect, 
                new EditorCustomUtilities.Section(20f, r =>
                {
                    var enable = EditorGUI.Toggle(r, indexProp.intValue == idx);
                    if (enable)
                        indexProp.intValue = idx;
                }), 
                new EditorCustomUtilities.Section(r =>
                {
                    EditorGUI.PropertyField(r, element, GUIContent.none);
                    var val = element.stringValue;
                }));
        }
        private ReorderableList OnEnable(SerializedProperty property, GUIContent label)
        {
            var selectedIndex = property.FindPropertyRelative("selectedIndex");
            var values = property.FindPropertyRelative("values");
            return new ReorderableList(property.serializedObject, values, true, true, true, true)
                {
                    drawHeaderCallback = rect => EditorGUI.LabelField(rect, label),
                    drawElementCallback = (rect, index, _, _) =>
                    {
                        DrawOptionElement(selectedIndex, values, rect, index);
                    },
                    elementHeight = EditorGUIUtility.singleLineHeight
                };
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            ReorderableList(property, label).DoList(position);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //var valuesProp = property.FindPropertyRelative("options");
            return EditorGUIUtility.singleLineHeight + ReorderableList(property, label).GetHeight();
        }
    }
}