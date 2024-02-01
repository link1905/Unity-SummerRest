using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PathContainer))]
    internal class PathContainerDrawer : UIToolkitDrawer
    {
        protected override string RelativeFromTemplateAssetPath => "Properties/path-container.uxml";
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = Tree;
            var valuesElement = tree.Q<PropertyField>("values");
            valuesElement.CallThenTrackPropertyValue(property, s =>
            {
                var valuesProp = s.FindPropertyRelative("values");
                var shouldShowSmartFields = valuesProp.arraySize > 0;
                valuesElement.Show(shouldShowSmartFields);
            });
            return tree;
        }
    }
}