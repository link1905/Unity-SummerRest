using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    /// <summary>
    /// Based UI drawers that need to load UXML files
    /// </summary>
    internal abstract class UIToolkitDrawer : PropertyDrawer
    {
        protected abstract string RelativeFromTemplateAssetPath { get;}
        /// <summary>
        /// When embed in Assets folder
        /// </summary>
        private const string AssetDir = "Assets/Plugins/SummerRest/Editor/Templates";
        /// <summary>
        /// When used as a package
        /// </summary>
        private const string PackageDir = "Packages/com.summer.summer-rest/Editor/Templates";
        
        public static string RootDir
        {
            get
            {
                // regular assets path
                if (AssetDatabase.IsValidFolder(PackageDir))
                    return PackageDir;
                return AssetDir;
            }
        }

        /// <summary>
        /// Caches UXML loaded file
        /// </summary>
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