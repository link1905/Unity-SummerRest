using SummerRest.Editor.Utilities;
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
        public static string RootDir
        {
            get
            {
                // regular assets path
                if (AssetDatabase.IsValidFolder(PathsHolder.PackageDir))
                    return PathsHolder.PackageDir;
                return PathsHolder.AssetDir;
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