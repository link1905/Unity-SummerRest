using SummerRest.Editor.Configurations;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Window.Elements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window
{
    public class SummerRestEditorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset domainManagersTemplate;
        [SerializeField] private VisualTreeAsset domainButtonTemplate;
        [SerializeField] private VisualTreeAsset endpointElementTemplate;
        [SerializeField] private VisualTreeAsset initializeGuideTemplate;
        [MenuItem("Tools/SummerRest")]
        public static void ShowExample()
        {
            SummerRestEditorWindow wnd = GetWindow<SummerRestEditorWindow>();
            wnd.titleContent = new GUIContent("SummerRest Configuration");
        }

        public void CreateGUI()
        {
            try
            {
                var conf = SummerRestConfiguration.Instance;
                var initializeGuideElement = domainManagersTemplate.Instantiate().Q<SummerRestConfigurationElement>();
                initializeGuideElement.Init(domainButtonTemplate, endpointElementTemplate, conf);
                rootVisualElement.Add(initializeGuideElement);
            }
            catch (SingletonException e)
            {
                switch (e.Count)
                {
                    case 0: // No configure is detected => show initializing panel
                        var initializeGuideElement = initializeGuideTemplate.Instantiate().Q<InitializeGuideElement>();
                        initializeGuideElement.Init();
                        rootVisualElement.Add(initializeGuideElement);
                        break;
                    case > 1: // More than 1 => show error
                        rootVisualElement.Add(new HelpBox($"There is more than 1 {nameof(SummerRestConfiguration)} in the project, please always keep one single instance asset of this class", HelpBoxMessageType.Error));
                        break;
                }
            }
        }
    }
}
