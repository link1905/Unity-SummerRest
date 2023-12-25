using System;
using System.Collections.Generic;
using System.Linq;
using SummerRest.Configurations;
using SummerRest.Editors.Utilities;
using SummerRest.Editors.Window.Elements;
using SummerRest.Models;
using SummerRest.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace SummerRest.Editors.Window
{
    public class SummerRestEditorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset tree;
        [SerializeField] private VisualTreeAsset domainButtonTemplate;
        [MenuItem("Tools/SummerRest")]
        public static void ShowExample()
        {
            SummerRestEditorWindow wnd = GetWindow<SummerRestEditorWindow>();
            wnd.titleContent = new GUIContent("SummerRest Configurations");
        }
        private void CheckData()
        {
            var configurationsManager = DomainConfigurationsManager.instance;
            var path = configurationsManager.GetAssetFolder();
            // Check the domain folder existing
            SummerRestAssetUtilities.CreateFolderIfNotExists(path, nameof(Domain));
        }
        public void Awake()
        {
            CheckData();
        }
        public void CreateGUI()
        {
            var root = rootVisualElement;
            var mainContainer = tree.Instantiate();
            DomainSection(mainContainer);
            SetupEndpointsTree(mainContainer);
            mainContainer.StretchToParentSize();
            root.Add(mainContainer);
        }
        private void DomainSection(VisualElement mainContainer)
        {
            var configurationsManager = DomainConfigurationsManager.instance;
            var domainsContainer = mainContainer.Q<VisualElement>("domains").Q<DomainListElement>();
            domainsContainer.Init(domainButtonTemplate, Color.gray);
            domainsContainer.OnDeleteElement += i =>
            {
                var domain = configurationsManager.Domains[i];
                if (!domain.RemoveAsset(SummerRestAssetUtilities.AskToRemoveMessage.RemoveDomain))
                    return false;
                configurationsManager.Domains.RemoveAt(i);
                return true;
            };
            domainsContainer.OnAdd += () =>
            {
                var path = configurationsManager.GetAssetFolder(nameof(Domain));
                var domain = SummerRestAssetUtilities.CreateAndSaveObject<Domain>(path);
                domain.EndpointName = "New domain";
                configurationsManager.Domains.Add(domain);
                return domain;
            };
            domainsContainer.OnElementClicked += i =>
            {
                var domain = configurationsManager.Domains[i];
                ShowEndpointsTree(mainContainer, domain);
            };
            foreach (var domain in configurationsManager.Domains)
                domainsContainer.AddChild(domain);
        }

        private void SetupEndpointsTree(VisualElement mainContainer)
        {
            var endpointContainer = mainContainer.Q<VisualElement>("endpoint-container");
            var endpointsTree = endpointContainer.Q<TreeView>("endpoint-tree");
            var endpointPropElement = endpointContainer.Q<PropertyField>("endpoint-prop");
            // var samples = new TreeViewItemData<string>[]
            // {
            //     new(0, "A", new List<TreeViewItemData<string>>
            //     {
            //         new(1, "B"),
            //         new(2, "C")
            //     }),
            //     new(3, "B", new List<TreeViewItemData<string>>
            //     {
            //         new(4, "A"),
            //         new(5, "D")
            //     })
            // };
            // endpointsTree.SetRootItems(samples);
            endpointsTree.makeItem = () =>
            {
                var label = new Label();
                //var color = Random.Range(0, 2) == 0 ? Color.yellow : Color.green;
                //label.style.ReplaceBackgroundColor(color);
                return label;
            };
            endpointsTree.bindItem = (element, index) =>
                (element as Label).text = endpointsTree.GetItemDataForIndex<Domain>(index).EndpointName;
            endpointsTree.selectionChanged += items =>
            {
                if (items.First() is not Domain domain)
                    return;
                var serializedObj = new SerializedObject(domain);
                
            };
        }
        private void ShowEndpointsTree(VisualElement mainContainer, Domain domain)
        {
            var endpointsTree = mainContainer.Q<VisualElement>("endpoint-container").Q<TreeView>("endpoint-tree");
            var treeView = domain.BuildChildrenTree(0);
            endpointsTree.SetRootItems(treeView);
        }
    }
}
