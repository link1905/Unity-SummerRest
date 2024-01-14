using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    internal abstract class UIToolkitDrawer : PropertyDrawer
    {
        public abstract string RelativeFromTemplateAssetPath { get;}
        private const string AssetDir = "Assets/Plugins/SummerRest/Editor/Templates";
        private const string PackageDir = "Packages/com.summer.summer-rest/Editor/Templates";
        
        public static string RootDir
        {
            get
            {
                // regular assets path
                string rootDir = "Assets/Plugins/SummerRest/Editor/Templates";
                // IMPORTANT: this is linked to the "name" property of the package manifest json file
                const string packagesDir = "Packages/com.summer/";
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
                _treeAsset ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{RootDir}/{RelativeFromTemplateAssetPath}");
                return _treeAsset.Instantiate();
            }
        }
    }
}