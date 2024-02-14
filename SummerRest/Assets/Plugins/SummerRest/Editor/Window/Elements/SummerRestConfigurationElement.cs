using System.Collections.Generic;
using System.Linq;
using SummerRest.Editor.Configurations;
using SummerRest.Editor.Manager;
using SummerRest.Editor.Models;
using SummerRest.Editor.Requests;
using SummerRest.Editor.Utilities;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    public class SummerRestConfigurationElement : VisualElement
    {
        private VisualTreeAsset _domainButtonTemplate;
        private VisualTreeAsset _endpointElementTemplate;
        private TreeView _endpointTree;
        private EndpointElement _endpointElement;
        private VisualElement _domainElement;
        private DomainListElement _domainListElement;
        private Domain _currentSelectedDomain;
        private SummerRestConfiguration _configuration;
        private SerializedObject _serializedConfiguration;
        public new class UxmlFactory : UxmlFactory<SummerRestConfigurationElement, UxmlTraits>
        {
        }
        private void ClearWindows()
        {
            ShowEndpointsTree(null);
            _endpointElement.UnBindAllChildren();
            _endpointElement.Show(false);
        }
   
        public void Init(VisualTreeAsset domainButtonTemplate, VisualTreeAsset endpointElementTemplate, SummerRestConfiguration configuration)
        {
            _domainButtonTemplate = domainButtonTemplate;
            _endpointElementTemplate = endpointElementTemplate;
            _configuration = configuration;
            _serializedConfiguration = new SerializedObject(_configuration);
            SetupDomainListSection();
            SetupEndpointsTree();
            SetupDomainActionsSection();
            SetupAuthSection();
            SetupGenerateSourceButton();
            ClearWindows();
        }
        /// <summary>
        /// Advanced settings on top of the window (only show when click on advanced settings button)
        /// </summary>
        private void SetupAuthSection()
        {
            var advancedToggle = this.Q<ToolbarToggle>("advanced");
            var advancedSettings = this.Q<VisualElement>("advanced-settings");
            advancedSettings.BindChildrenToProperties(_serializedConfiguration);
            advancedSettings.Show(advancedToggle.value);
            advancedToggle.RegisterValueChangedCallback(e => {
                advancedSettings.Show(e.newValue);
            });
        }
        
        /// <summary>
        /// Generating source section since the automation of producing JSON file is much heavy, so I make a section to let user do it manually 
        /// </summary>
        private void SetupGenerateSourceButton()
        {
            var targetAssembly = this.Q<ObjectField>("target-assembly");
            targetAssembly.BindProperty(_serializedConfiguration);
            var genBtn = this.Q<ToolbarButton>("gen-btn");
            genBtn.clicked += SourceGenerator.GenerateAdditionalFile;
        }
    
        /// <summary>
        /// Show a list of all domains on top of the window (under the advanced section)
        /// </summary>
        private void SetupDomainListSection()
        {
            var domains = _configuration.Domains;
            var domainsContainer = this.Q<VisualElement>("domains").Q<DomainListElement>();
            _domainListElement = domainsContainer;
            _domainListElement.Init(_domainButtonTemplate, Color.gray);
            _domainListElement.OnDeleteElement += (i, isSelected) =>
            {
                var domain = domains[i];
                return DeleteDomain(domain, isSelected);
            };
            _domainListElement.OnElementClicked += OnSelectDomain;
            foreach (var domain in domains)
                _domainListElement.AddChild(domain, false);
        }

        private bool DeleteDomain(Domain domain, bool isSelected)
        {
            var domains = _configuration.Domains;
            if (!domain.AskToRemoveAsset(d =>
                {
                    domains.Remove(d);
                    d.Delete(false);
                }, EditorAssetUtilities.AskToRemoveMessage.RemoveDomain))
                return false;
            if (isSelected)
                ClearWindows();
            _configuration.RenameAssets();
            return true;
        }

        private Domain OnAddDomain()
        {
            EditorAssetUtilities.CreateFolderIfNotExists(_configuration.GetAssetFolder(), "Domains");
            var path = _configuration.GetAssetFolder("Domains");
            var domain = EditorAssetUtilities.CreateAndSaveObject<Domain>(path);
            _configuration.Domains.Add(domain);
            domain.Domain = domain;
            domain.EndpointName = $"Domain {_configuration.Domains.Count}";
            domain.MakeDirty();
            _configuration.RenameAssets();
            return domain;
        }

        private void OnSelectDomain(int domainIndex)
        {
            var domain = _configuration.Domains[domainIndex];
            if (domain == _currentSelectedDomain)
                return;
            ShowEndpointsTree(domain);
            ShowDomainAction(domain);
        }
        private void SetupDomainActionsSection()
        {
            _domainElement = this
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
                _ => {
                {
                    AddEndpointToCurrentDomain(ElementAddAction.Service);
                } }
            );
            addMenu.menu.AppendAction(
                ElementAddAction.Request.ToString(),
                _ => { AddEndpointToCurrentDomain(ElementAddAction.Request); }
            );
            var searchField = _domainElement.Q<ToolbarSearchField>();
            searchField.RegisterValueChangedCallback(e =>
            {
                if (_currentSelectedDomain is null || e.newValue is null)
                    return;
                FindEndpoint(e.newValue);
            });
        }

        private void AddEndpointToCurrentDomain(ElementAddAction elementAddAction)
        {
            if (_currentSelectedDomain is null)
            {
                EditorUtility.DisplayDialog("No domain detected!", "You must select or create a domain first",
                    "Ok");
                return;
            }
            OnEndpointElementAddChild(elementAddAction, _currentSelectedDomain);
        }
        private void SetupEndpointsTree()
        {
            var endpointContainer = this.Q<VisualElement>("endpoint-container");
            _endpointTree = endpointContainer.Q<VisualElement>("domain").Q<TreeView>("endpoint-tree");
            _endpointElement = endpointContainer.Q<EndpointElement>();
            _endpointElement.Init();
            _endpointElement.OnRequest += (e, callback) =>
            {
                EditorCoroutineUtility.StartCoroutine(EditorRequest.Create(e).MakeRequest(callback), this);
            };
            _endpointTree.makeItem = () =>
            {
                var endpointTreeElement = _endpointElementTemplate.Instantiate().Q<EndpointTreeElement>();
                endpointTreeElement.OnAddChild += OnEndpointElementAddChild;
                endpointTreeElement.OnDelete += OnEndpointElementRemoveChild;
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
                if (items.First() is not Endpoint selectedEndpoint)
                    return;
                ShowEndpoint(selectedEndpoint);
            };
            _endpointTree.itemIndexChanged += (drag, drop) =>
            {
                var dragEndpoint = _endpointTree.GetItemDataForId<Endpoint>(drag);
                if (_endpointTree.GetItemDataForId<Endpoint>(drop) is EndpointContainer dropEndpoint)
                {
                    // Dropped element can not be child of the dragged element
                    if (!dropEndpoint.IsChildOf(dragEndpoint))
                    {
                        dropEndpoint.AddEndpoint(dragEndpoint);
                        _configuration.RenameAssets();
                    }
                }
                ShowEndpointsTree(_currentSelectedDomain);
                _endpointTree.SetSelectionById(dragEndpoint.TreeId);
            };
        }

        private void ShowEndpoint(Endpoint endpoint)
        {
            _endpointElement.Show(true);
            _endpointElement.ShowEndpoint(endpoint);
        }
        private void FindEndpoint(string search)
        {
            foreach (var id in _endpointTree.GetRootIds())
            {
                var endpoint = _endpointTree.GetItemDataForId<Endpoint>(id);
                if (endpoint.Url.Contains(search))
                {
                    _endpointTree.SetSelectionById(endpoint.TreeId);
                    return;
                }
            }
        }
        private void ShowDomainAction(Domain domain)
        {
            _currentSelectedDomain = domain;
            _endpointTree.SetSelectionById(domain.TreeId);
        }
        private void ShowEndpointsTree(Domain domain)
        {
            var treeView = domain is not null ? new List<TreeViewItemData<Endpoint>>
                {
                    domain.BuildTree(0)
                } 
                : new List<TreeViewItemData<Endpoint>>();
            _endpointTree.SetRootItems(treeView);
            _endpointTree.Rebuild();
        }
   
        private void OnEndpointElementAddChild(ElementAddAction elementAddAction, Endpoint endpoint)
        {
            if (endpoint is not EndpointContainer endpointContainer) 
                return;
            var path = _configuration.GetAssetFolder("Domains");
            switch (elementAddAction)
            {
                case ElementAddAction.Service:
                    endpoint = EditorAssetUtilities.CreateAndSaveObject<Service>(path);
                    break;
                case ElementAddAction.Request:
                    endpoint = EditorAssetUtilities.CreateAndSaveObject<Request>(path);
                    break;
                default:
                    return;
            }
            endpointContainer.AddEndpoint(endpoint);
            _configuration.RenameAssets();

            // Update the endpoint tree
            ShowEndpointsTree(_currentSelectedDomain);
            _endpointTree.SetSelectionById(endpoint.TreeId);
            //Unity UIToolkit bug that not fire onSelectionChange event
            ShowEndpoint(endpoint);
        }
        private void OnEndpointElementRemoveChild(Endpoint endpoint)
        {
            if (endpoint is Domain domain)
            {
                _domainListElement.DeleteChild(domain);
                return;
            }
            if (endpoint is null || endpoint.Parent is null)
                return;
            var currentSelection = _endpointTree.selectedItem as Endpoint;
            // If current selection is the deleted candidate 
            if (currentSelection == endpoint)
                currentSelection = null;
            switch (endpoint)
            {
                case Service service:
                    if (!service.AskToRemoveAsset(deleteAction: s => s.Delete(true), EditorAssetUtilities.AskToRemoveMessage.RemoveService))
                        return;
                    break;
                case Request request:
                    request.Delete(true);
                    break;
                default:
                    return;
            }
            _configuration.RenameAssets();

            ClearWindows();
            ShowEndpointsTree(_currentSelectedDomain);
            // Show the previous selection
            if (currentSelection is not null)
            {
                _endpointTree.SetSelectionById(currentSelection.TreeId);
            }
        }
    }
}