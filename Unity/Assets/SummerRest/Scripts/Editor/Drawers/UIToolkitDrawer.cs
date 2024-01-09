using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    internal abstract class UIToolkitDrawer : PropertyDrawer
    {
        public abstract string AssetPath { get;} 
        //private const string AssetPath = "Assets/SummerRest/Editors/Templates/Properties/inherit-or-custom.uxml";
        public static string RootDir
        {
            get
            {
                // regular assets path
                string rootDir = "Assets/whatever/";
 
                // IMPORTANT: this is linked to the "name" property of the package manifest json file
                string packagesDir = "Packages/com.whatever/";
 
                if (AssetDatabase.IsValidFolder(packagesDir))
                    rootDir = packagesDir;
 
                return rootDir;
            }
        }
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