using System.Collections.Generic;
using SummerRest.Editors.Window.Elements;
using SummerRest.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window
{
    public class SummerRestEditorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset tree;
        [SerializeField] private VisualTreeAsset domainButtonTemplate;
        [MenuItem("Window/SummerRestEditorWindow")]
        public static void ShowExample()
        {
            SummerRestEditorWindow wnd = GetWindow<SummerRestEditorWindow>();
            wnd.titleContent = new GUIContent("SummerRestEditorWindow");
        }
        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;
            var mainContainer = tree.Instantiate();
            DomainSection(mainContainer);
            EndpointsTree(mainContainer);
            mainContainer.StretchToParentSize();
            root.Add(mainContainer);
        }
        private void DomainSection(VisualElement mainContainer)
        {
            var domainsContainer = mainContainer.Q<VisualElement>("domains").Q<IndexedButtonListElement>();
            domainsContainer.Init(domainButtonTemplate, Color.gray);
            var testDomains = new[]
            {
                "ABC", "Example.com", "DDS"
            };
            foreach (var domain in testDomains)
            {
                domainsContainer.AddChild(domain);
            }
        }
        private void EndpointsTree(VisualElement mainContainer)
        {
            var endpointsTree = mainContainer.Q<VisualElement>("endpoint-container").Q<TreeView>("endpoint-tree");
            var samples = new TreeViewItemData<string>[]
            {
                new(0, "A", new List<TreeViewItemData<string>>
                {
                    new(1, "B"),
                    new(2, "C")
                }),
                new(3, "B", new List<TreeViewItemData<string>>
                {
                    new(4, "A"),
                    new(5, "D")
                })
            };
            // Call TreeView.SetRootItems() to populate the data in the tree.
            endpointsTree.SetRootItems(samples);
            // Set TreeView.makeItem to initialize each node in the tree.
            endpointsTree.makeItem = () =>
            {
                var label = new Label();
                var color = Random.Range(0, 2) == 0 ? Color.yellow : Color.green;
                label.style.ReplaceBackgroundColor(color);
                return label;
            };
            // Set TreeView.bindItem to bind an initialized node to a data item.
            endpointsTree.bindItem = (element, index) =>
                (element as Label).text = endpointsTree.GetItemDataForIndex<string>(index);
        }
    }
}
