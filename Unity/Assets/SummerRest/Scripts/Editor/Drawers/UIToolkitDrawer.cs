using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    internal abstract class UIToolkitDrawer : PropertyDrawer
    {
        public abstract string AssetPath { get;} 
        //private const string AssetPath = "Assets/SummerRest/Editors/Templates/Properties/inherit-or-custom.uxml";
        private VisualTreeAsset _treeAsset;
        protected VisualElement Tree
        {
            get
            {
                _treeAsset ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetPath);
                return _treeAsset.Instantiate();
            }
        }
    }
}