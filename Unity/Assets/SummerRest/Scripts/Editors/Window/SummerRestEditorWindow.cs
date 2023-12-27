using System.Collections.Generic;
using System.Linq;
using SummerRest.Configurations;
using SummerRest.Editors.Window.Elements;
using SummerRest.Models;
using SummerRest.Scripts.Utilities.Editor;
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
        private VisualElement _mainContainer;
        private TreeView _endpointTree;
        private EndpointElement _endpointElement;
        private VisualElement _domainElement;
        private DomainListElement _domainListElement;

        [MenuItem("Tools/SummerRest")]
        public static void ShowExample()
        {
            SummerRestEditorWindow wnd = GetWindow<SummerRestEditorWindow>();
            wnd.titleContent = new GUIContent("SummerRest Configurations");
        }
        private void CheckDataExisting()
        {
            var configurationsManager = DomainConfigurationsManager.Instance;
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
            _mainContainer = tree.Instantiate();
            SetupDomainListSection();
            SetupEndpointsTree();
            SetupDomainSection();
            _mainContainer.StretchToParentSize();
            root.Add(_mainContainer);
        }
        
        // Setup domain list on top of the window
        private void SetupDomainListSection()
        {
            var configurationsManager = DomainConfigurationsManager.Instance;
            var domains = configurationsManager.Domains;
            var domainsContainer = _mainContainer.Q<VisualElement>("domains").Q<DomainListElement>();
            _domainListElement = domainsContainer;
            _domainListElement.Init(domainButtonTemplate, Color.gray);
            _domainListElement.OnDeleteElement += (i, isSelected) =>
            {
                var domain = domains[i];
                if (!domain.AskToRemoveAsset(d =>
                    {
                        domains.Remove(d);
                        d.Delete(false);
                    }, SummerRestAssetUtilities.AskToRemoveMessage.RemoveDomain))
                    return false;
                if (isSelected)
                    ClearWindows();
                configurationsManager.MakeDirty();
                return true;
            };
            // _domainListElement.OnAdd += OnAddDomain;
            _domainListElement.OnElementClicked += OnSelectDomain;
            foreach (var domain in domains)
                _domainListElement.AddChild(domain, false);
        }

        private Domain OnAddDomain()
        {
            var configurationsManager = DomainConfigurationsManager.Instance;
            var path = configurationsManager.GetAssetFolder(nameof(Domain));
            var domain = SummerRestAssetUtilities.CreateAndSaveObject<Domain>(path);
            configurationsManager.Domains.Add(domain);
            domain.EndpointName = $"Domain {configurationsManager.Domains.Count}";
            configurationsManager.MakeDirty();
            return domain;
        }
        private void OnSelectDomain(int domainIndex)
        {
            var domain = DomainConfigurationsManager.Instance.Domains[domainIndex];
            if (domain == _currentSelectedDomain)
                return;
            ShowEndpointsTree(domain, string.Empty);
            ShowDomainAction(domain);
        }
        // Setup domain search section
        private void SetupDomainSection()
        {
            _domainElement = _mainContainer
                .Q<VisualElement>("endpoint-container")
                .Q<VisualElement>("domain");
            var addMenu = _domainElement.Q<ToolbarMenu>("add-menu");
            addMenu.menu.AppendAction(nameof(Domain), _ =>
            {
                var domain = OnAddDomain();
                _domainListElement.AddChild(domain, true);
            });
            addMenu.menu.AppendAction(
                ElementAddAction.Service.ToString(),
                _ => { OnEndpointElementOnOnAddChild(ElementAddAction.Service, _currentSelectedDomain); }
            );
            addMenu.menu.AppendAction(
                ElementAddAction.Request.ToString(),
                _ => { OnEndpointElementOnOnAddChild(ElementAddAction.Request, _currentSelectedDomain); }
            );
            var searchField = _domainElement.Q<ToolbarSearchField>();
            searchField.RegisterValueChangedCallback(e =>
            {
                if (_currentSelectedDomain is null || e.newValue is null)
                    return;
                FindEndpoint(e.newValue);
            });
        }
        private void SetupEndpointsTree()
        {
            var endpointContainer = _mainContainer.Q<VisualElement>("endpoint-container");
            _endpointTree = endpointContainer.Q<VisualElement>("domain").Q<TreeView>("endpoint-tree");
            _endpointElement = endpointContainer.Q<EndpointElement>();
            _endpointElement.Init();
            _endpointTree.makeItem = () =>
            {
                var endpointTreeElement = endpointElementTemplate.Instantiate().Q<EndpointTreeElement>();
                endpointTreeElement.OnAddChild += OnEndpointElementOnOnAddChild;
                endpointTreeElement.OnDelete += OnEndpointElementOnOnRemoveChild;
                return endpointTreeElement;
            };
            _endpointTree.bindItem = (element, index) =>
            {
                if (element is not EndpointTreeElement endpointElement)
                    return;
                endpointElement.Init(_endpointTree.GetItemDataForIndex<Endpoint>(index));
            };
            _endpointTree.selectionChanged += items =>
            {
                if (items.First() is not Endpoint domain)
                    return;
                _endpointElement.Init(domain);
            };
        }
        private void FindEndpoint(string search)
        {
            foreach (var id in _endpointTree.GetRootIds())
            {
                var endpoint = _endpointTree.GetItemDataForId<Endpoint>(id);
                if (endpoint.Path.Contains(search))
                {
                    _endpointTree.ScrollToItemById(id);
                    return;
                }
            }
        }

        private void ShowDomainAction(Domain domain)
        {
            _currentSelectedDomain = domain;
        }
        private void ShowEndpointsTree(Domain domain, string search)
        {
            var treeView = domain is not null ? domain.BuildChildrenTree(0, search) : new List<TreeViewItemData<Endpoint>>();
            _endpointTree.SetRootItems(treeView);
            _endpointTree.Rebuild();
        }
        
        private void ClearWindows()
        {
            ShowEndpointsTree(null, null);
            _endpointElement.UnBindAllChildren();
        }
        private void OnEndpointElementOnOnAddChild(ElementAddAction elementAddAction, Endpoint endpoint)
        {
            if (endpoint is not EndpointContainer parent) 
                return;
            var configurationsManager = DomainConfigurationsManager.Instance;
            var path = configurationsManager.GetAssetFolder(nameof(Domain));
            switch (elementAddAction)
            {
                case ElementAddAction.Service:
                    var service = SummerRestAssetUtilities.CreateAndSaveObject<Service>(path);
                    var idx = parent.AddService(service);
                    service.EndpointName = $"Service {idx}";
                    break;
                case ElementAddAction.Request:
                    var request = SummerRestAssetUtilities.CreateAndSaveObject<Request>(path);
                    idx = parent.AddRequest(request);
                    request.EndpointName = $"Request {idx}";
                    break;
                default:
                    return;
            }
            parent.MakeDirty();
            // Update the endpoint tree
            ShowEndpointsTree(_currentSelectedDomain, string.Empty);
        }
        private void OnEndpointElementOnOnRemoveChild(Endpoint endpoint)
        {
            if (endpoint is null || endpoint.Parent is not EndpointContainer parent)
                return;
            switch (endpoint)
            {
                case Service service:
                    if (!service.AskToRemoveAsset(deleteAction: s => s.Delete(true), SummerRestAssetUtilities.AskToRemoveMessage.RemoveService))
                        return;
                    break;
                case Request request:
                    request.Delete(true);
                    break;
                default:
                    return;
            }
            parent.MakeDirty();
            // Update the endpoint tree
            ShowEndpointsTree(_currentSelectedDomain, string.Empty);
        }
    }
}
