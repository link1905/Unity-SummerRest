using SummerRest.Editors.Utilities;
using SummerRest.Models;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editors.Drawers
{
    // [CustomPropertyDrawer(typeof(Domain))]
    // Pending
    public class DomainDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return;
            var nameProp = property.FindBackingPropertyRelative(nameof(Domain.Name));
            EditorGUILayout.PropertyField(nameProp, includeChildren: true);
        }
    }
}