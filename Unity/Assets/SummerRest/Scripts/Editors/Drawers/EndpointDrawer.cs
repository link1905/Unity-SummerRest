using SummerRest.Models;
using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Drawers
{
    [CustomPropertyDrawer(typeof(EndPoint), true)]
    public class EndpointDrawer : PropertyDrawer
    {        
        private const string AssetPath = "Assets/SummerRest/Editors/Templates/Properties/endpoint_UXML.uxml";
        private VisualTreeAsset _treeAsset;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _treeAsset ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetPath);
            var tree = _treeAsset.Instantiate();

            var parentPath = property.FindPropertyRelative("parentPath");
            var nameElement = tree.Q<TextField>("name");
            nameElement.label = parentPath.stringValue;
            //nameElement.BindProperty(nameProp);
            return tree;
        }
    }
}