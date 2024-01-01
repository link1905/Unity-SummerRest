using SummerRest.Editor.Utilities;
using SummerRest.Scripts.Utilities.DataStructures;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(OptionsArray<>))]
    internal class OptionsArrayDrawer : PropertyDrawer
    {
        private ReorderableList _reorderableList;
        private float _lastPosWidth;
        private ReorderableList ReorderableList(SerializedProperty property, GUIContent label)
        {
            return _reorderableList ??= OnEnable(property, label);
        }
        private void DrawOptionElement(SerializedProperty indexProp, SerializedProperty valuesProp, Rect rect, int idx)
        {
            var element = valuesProp.GetArrayElementAtIndex(idx);
            using var scope = SummerRestEditorUtilities.LayoutOptions.EditorGUIDrawHorizontalLayout.Create(rect);
            var enable = scope.Toggle(indexProp.intValue == idx, 20f);
            if (enable)
                indexProp.intValue = idx;
            scope.PropertyField(element, label: GUIContent.none);
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
                        rect.width = _lastPosWidth = Mathf.Max(rect.width, _lastPosWidth);
                        DrawOptionElement(selectedIndex, values, rect, index);
                    },
                    elementHeight = SummerRestEditorUtilities.Sizes.SingleLineHeight,
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
            var list = ReorderableList(property, label);
            return list.GetHeight();
        }
    }
}