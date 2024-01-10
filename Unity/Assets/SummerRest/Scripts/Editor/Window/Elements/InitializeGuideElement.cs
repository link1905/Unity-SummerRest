using System;
using SummerRest.Editor.Configurations;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    public class InitializeGuideElement : VisualElement
    {
        private Button _openFolderBtn;
        private Button _initializeBtn;
        private TextField _pathField;
        public new class UxmlFactory : UxmlFactory<InitializeGuideElement, UxmlTraits>
        {
        }
        public void Init()
        {
            _openFolderBtn = this.Q<Button>("open-folder");
            _initializeBtn = this.Q<Button>("initialize");
            _pathField = this.Q<TextField>("path");
            _initializeBtn.SetEnabled(false);
            _initializeBtn.clicked += InitAssets;
            _openFolderBtn.clicked += () =>
            {
                var folderPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "SummerRestConfiguration");
                var valid = !string.IsNullOrEmpty(folderPath) && AssetDatabase.IsValidFolder(folderPath); 
                _pathField.value = folderPath;
                _initializeBtn.SetEnabled(valid);
            };
        }

        private void InitAssets()
        {
            try
            {
                _ = SummerRestConfiguration.Instance;
            }
            catch (SingletonException e)
            {
                if (e.Count != 0)
                    return;
                var path = _pathField.value;
                if (!AssetDatabase.IsValidFolder(path))
                    return;
                var conf = EditorAssetUtilities.CreateAndSaveObject<SummerRestConfiguration>(nameof(SummerRestConfiguration), path);
                conf.AuthenticateConfiguration = EditorAssetUtilities.CreateAndSaveObject<AuthenticateConfiguration>(nameof(AuthenticateConfiguration), path);
                conf.MakeDirty();
            }
        }
    }
}