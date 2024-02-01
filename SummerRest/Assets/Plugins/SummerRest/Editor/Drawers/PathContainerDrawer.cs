using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using UnityEditor;
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
            var formatElement = tree.Q<Foldout>("format-values");
            formatElement.CallThenTrackPropertyValue(property, s =>
            {
                var valuesProp = s.FindPropertyRelative("values");
                var shouldShowSmartFields = valuesProp.arraySize > 0;
                formatElement.Show(shouldShowSmartFields);
            });
            return tree;
        }
    }
}