using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    internal abstract class UIToolkitDrawer : PropertyDrawer
    {
        public abstract string RelativeFromTemplateAssetPath { get;} 
        public static string RootDir => "Assets/Plugins/SummerRest/Templates";
        private VisualTreeAsset _treeAsset;
        protected VisualElement Tree
        {
            get
            {
                _treeAsset ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{RootDir}/{RelativeFromTemplateAssetPath}");
                return _treeAsset.Instantiate();
            }
        }
    }
}