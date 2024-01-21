using SummerRest.Editor.Configurations;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    /// <summary>
    /// The element will be shown when no <see cref="SummerRestConfiguration"/> is detected in the project
    /// </summary>
    public class InitializeGuideElement : VisualElement
    {
        private ObjectField _folder;
        private Button _initializeBtn;
        public new class UxmlFactory : UxmlFactory<InitializeGuideElement, UxmlTraits>
        {
        }
        public void Init()
        {
            _folder = this.Q<ObjectField>("folder");
            _initializeBtn = this.Q<Button>("init");
            _initializeBtn.SetEnabled(false);
            _initializeBtn.clicked += InitAssets;
            _folder.RegisterValueChangedCallback(change =>
            {
                var obj = change.newValue;
                var notValid = obj is null || !AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(obj));
                _initializeBtn.SetEnabled(!notValid);
                if (notValid)
                    _folder.SetValueWithoutNotify(null);
            });
        }

        private void InitAssets()
        {
            try
            {
                _ = SummerRestConfiguration.Instance;
            }
            catch (SingletonException e)
            {
                if (e.Count != 0 || _folder.value is null)
                    return;
                var folder = AssetDatabase.GetAssetPath(_folder.value);
                if (!AssetDatabase.IsValidFolder(folder))
                    return;
                var conf = EditorAssetUtilities.CreateAndSaveObject<SummerRestConfiguration>(nameof(SummerRestConfiguration), folder);
                conf.MakeDirty();
                EditorUtility.RequestScriptReload();
            }
        }
    }
}