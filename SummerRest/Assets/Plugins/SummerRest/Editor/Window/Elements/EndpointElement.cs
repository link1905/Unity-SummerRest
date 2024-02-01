using System;
using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    /// <summary>
    /// Generic endpoint's view section of <see cref="SummerRestConfigurationElement"/> behaves differently depended on the type of end point (<see cref="Service"/>,<see cref="Domain"/>,<see cref="Request"/>)
    /// </summary>
    public class EndpointElement : VisualElement
    {
        public event Action<Request, Action> OnRequest;
        private Endpoint _endpoint;
        private Foldout _advancedSettingsFoldout;
        private int _sharedElementsOriginalIndex;
        private VisualElement _sharedElements;
        private TextField _nameElement;
        private PropertyField _pathElement;
        private TextField _urlElement;
        private VisualElement _requestBodyElement;
        private Button _requestBtn;

        public new class UxmlFactory : UxmlFactory<EndpointElement, UxmlTraits>
        {
        }
 
        private void OnClick()
        {
            if (_endpoint is not Request request)
                return;
            _requestBtn.SetEnabled(false);
            OnRequest?.Invoke(request, () => _requestBtn.SetEnabled(true));
        }

        public void Init()
        {
            _requestBodyElement = this.Q<VisualElement>("request-body-element");
             _requestBtn = _requestBodyElement.Q<Button>("request-btn");
            _requestBtn.clicked += OnClick;
            _advancedSettingsFoldout = this.Q<Foldout>("advanced-settings");
            _sharedElements = this.Q<VisualElement>("shared-elements");
            _nameElement = this.Q<TextField>("name");
            _pathElement = this.Q<PropertyField>("path");
            _urlElement = this.Q<TextField>("url");
            _sharedElementsOriginalIndex = IndexOf(_sharedElements);
        }
        /// <summary>
        /// Try to show only beneficial fields (the others will be moved to the advanced section)
        /// </summary>
        /// <param name="shouldBeMovedToAdvancedPart"></param>
        private void ShowAdvancedSettings(bool shouldBeMovedToAdvancedPart)
        {
            _advancedSettingsFoldout.Show(shouldBeMovedToAdvancedPart);
            if (shouldBeMovedToAdvancedPart)
                _advancedSettingsFoldout.Add(_sharedElements);
            else
                Insert(_sharedElementsOriginalIndex, _sharedElements);
        }

        private void UnbindAll()
        {
            this.UnBindAllChildren();
        }
        
        public void ShowEndpoint(Endpoint endpoint)
        {
            UnbindAll();
            _endpoint = endpoint;
            var isRequest = !endpoint.IsContainer;
            var serializedObj = new SerializedObject(endpoint);
            _nameElement.label = endpoint.TypeName;
            _requestBodyElement.Show(isRequest);
            // If domain => use activeVersion instead of path
            var isDomain = endpoint is Domain;
            _pathElement.label = isDomain ? "Active version" : "Relative path";
            _pathElement.SetEnabled(!isDomain);
            // If request => urlWithParams
            _urlElement.SetEnabled(false);
            _urlElement.bindingPath = endpoint is Request ? "urlWithParam" : "url";
            ShowAdvancedSettings(isRequest);
            this.BindChildrenToProperties(serializedObj);
        }
    }
}