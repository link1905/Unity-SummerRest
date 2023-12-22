using System.Collections.Generic;
using SummerRest.Configurations;
using SummerRest.Editors.Window.Elements;
using SummerRest.Models;
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
            var configurationsManager = DomainConfigurationsManager.instance;
            DomainSection(mainContainer, configurationsManager);
            EndpointsTree(mainContainer, configurationsManager);
            mainContainer.StretchToParentSize();
            root.Add(mainContainer);
        }
        private void DomainSection(VisualElement mainContainer, DomainConfigurationsManager configurationsManager)
        {
            var domainsContainer = mainContainer.Q<VisualElement>("domains").Q<DomainListElement>();
            domainsContainer.Init(domainButtonTemplate, Color.gray);
            domainsContainer.OnDeleteElement += i =>
            {
                configurationsManager.Domains.RemoveAt(i);
                return true;
            };
            domainsContainer.OnAdd += () =>
            {
                var newDomain = new Domain();
                configurationsManager.Domains.Add(newDomain);
                return newDomain;
            };
            foreach (var domain in configurationsManager.Domains)
            {
                domainsContainer.AddChild(domain);
            }
        }
        private void EndpointsTree(VisualElement mainContainer, DomainConfigurationsManager configurationsManager)
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
