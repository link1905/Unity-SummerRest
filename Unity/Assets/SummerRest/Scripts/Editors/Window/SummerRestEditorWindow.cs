using System;
using System.Linq;
using SummerRest.Configurations;
using SummerRest.Editors.Utilities;
using SummerRest.Editors.Window.Elements;
using SummerRest.Models;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window
{
    public class SummerRestEditorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset tree;
        [SerializeField] private VisualTreeAsset domainButtonTemplate;
        [SerializeField] private VisualTreeAsset endpointElementTemplate;
        private Domain _currentSelectedDomain;

        [MenuItem("Tools/SummerRest")]
        public static void ShowExample()
        {
            SummerRestEditorWindow wnd = GetWindow<SummerRestEditorWindow>();
            wnd.titleContent = new GUIContent("SummerRest Configurations");
        }
        private void CheckDataExisting()
        {
            var configurationsManager = DomainConfigurationsManager.instance;
            var path = configurationsManager.GetAssetFolder();
            // Check the domain folder existing
            SummerRestAssetUtilities.CreateFolderIfNotExists(path, nameof(Domain));
        }
        public void Awake()
        {
            CheckDataExisting();
        }
        public void CreateGUI()
        {
            var root = rootVisualElement;
            var mainContainer = tree.Instantiate();
            SetupDomainListSection(mainContainer);
            SetupEndpointsTree(mainContainer);
            SetupDomainSection(mainContainer);
            mainContainer.StretchToParentSize();
            root.Add(mainContainer);
        }
        // Setup domain list on top of the window
        private void SetupDomainListSection(VisualElement mainContainer)
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
                ShowEndpointsTree(mainContainer, domain, string.Empty);
                ShowDomainAction(mainContainer, domain);
            };
            foreach (var domain in configurationsManager.Domains)
                domainsContainer.AddChild(domain);
        }
        // Setup domain search section
        private void SetupDomainSection(VisualElement mainContainer)
        {
            var domainElement = mainContainer
                .Q<VisualElement>("domain")
                .Q<VisualElement>("endpoint-container")
                .Q<VisualElement>("domain");
                        
            var addServiceBtn = domainElement.Q<Button>("add-service");
            var addRequestBtn = domainElement.Q<Button>("add-request");
            domainElement.style.Show(true);
            var searchField = domainElement.Q<ToolbarSearchField>();
            searchField.RegisterValueChangedCallback(e =>
            {
                if (_currentSelectedDomain is null)
                    return;
                ShowEndpointsTree(mainContainer, _currentSelectedDomain, e.newValue);
            });
            addServiceBtn.clicked += () =>
            {
                OnEndpointElementOnOnAddChild(ElementAddAction.Service, _currentSelectedDomain);
            };
            addRequestBtn.clicked += () =>
            {
                OnEndpointElementOnOnAddChild(ElementAddAction.Request, _currentSelectedDomain);
            };
        }
        private void SetupEndpointsTree(VisualElement mainContainer)
        {
            var endpointContainer = mainContainer.Q<VisualElement>("endpoint-container");
            var endpointsTree = endpointContainer.Q<VisualElement>("domain").Q<TreeView>("endpoint-tree");
            var endpointPropElement = endpointContainer.Q<PropertyField>("endpoint-prop");
            
            endpointsTree.makeItem = () =>
            {
                var endpointElement = endpointElementTemplate.Instantiate();
                return endpointElement;
            };
            endpointsTree.bindItem = (element, index) =>
            {
                if (element is not EndpointElement endpointElement)
                    return;
                endpointElement.Init(endpointsTree.GetItemDataForIndex<Endpoint>(index));
                endpointElement.OnAddChild += OnEndpointElementOnOnAddChild;
                endpointElement.OnDelete += OnEndpointElementOnOnRemoveChild;
            };
            endpointsTree.selectionChanged += items =>
            {
                if (items.First() is not Endpoint domain)
                    return;
                var serializedObj = new SerializedObject(domain);
                endpointPropElement.Unbind();
                endpointPropElement.Bind(serializedObj);
            };
        }
        
        private void ShowDomainAction(VisualElement mainContainer, Domain domain)
        {
            var domainElement = mainContainer
                .Q<VisualElement>("domain")
                .Q<VisualElement>("endpoint-container")
                .Q<VisualElement>("domain");
            domainElement.style.Show(true);
            _currentSelectedDomain = domain;
        }
        private void ShowEndpointsTree(VisualElement mainContainer, Domain domain, string search)
        {
            var endpointsTree = mainContainer.Q<VisualElement>("endpoint-container").Q<TreeView>("endpoint-tree");
            var treeView = domain.BuildChildrenTree(0, search);
            endpointsTree.SetRootItems(treeView);
        }
        private void OnEndpointElementOnOnAddChild(ElementAddAction elementAddAction, Endpoint endpoint)
        {
            if (endpoint is not EndpointContainer endpointContainer) 
                return;
            var configurationsManager = DomainConfigurationsManager.instance;
            var path = configurationsManager.GetAssetFolder(nameof(Domain));
            switch (elementAddAction)
            {
                case ElementAddAction.Service:
                    var service = SummerRestAssetUtilities.CreateAndSaveObject<Service>(path);
                    endpointContainer.Services.Add(service);
                    break;
                case ElementAddAction.Request:
                    var request = SummerRestAssetUtilities.CreateAndSaveObject<Request>(path);
                    endpointContainer.Requests.Add(request);
                    break;
                default:
                    return;
            }
        }
        private void OnEndpointElementOnOnRemoveChild(Endpoint endpoint)
        {
            if (endpoint is null || endpoint.Parent is not EndpointContainer parent)
                return;
            switch (endpoint)
            {
                case Service service:
                    if (!service.RemoveAsset(SummerRestAssetUtilities.AskToRemoveMessage.RemoveService))
                        return;
                    parent.Services.Remove(service);
                    break;
                case Request request:
                    request.RemoveAsset();
                    parent.Requests.Remove(request);
                    break;
                default:
                    return;
            }
        }

    }
}
