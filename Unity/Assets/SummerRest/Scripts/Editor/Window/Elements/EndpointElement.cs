using System;
using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    public class EndpointElement : VisualElement
    {
        public event Action<Endpoint> OnRequest;
        private Endpoint _endpoint;
        private Foldout _advancedSettingsFoldout;
        private int _sharedElementsOriginalIndex;
        private VisualElement _sharedElements;
        private TextField _nameElement;
        private TextField _pathElement;
        private TextField _urlElement;
        private TextOrCustomDataElement _requestBodyElement; 
        private ResponseElement _responseElement; 
        public new class UxmlFactory : UxmlFactory<EndpointElement, UxmlTraits>
        {
        }
        public EndpointElement()
        {
        }

        private void OnClick()
        {
            if (_endpoint is null)
                return;
            OnRequest?.Invoke(_endpoint);
        }

        public void Init()
        {
            _requestBodyElement = this.Q<TextOrCustomDataElement>();
             var requestBtn = _requestBodyElement.Q<Button>("request-btn");
            requestBtn.clicked += OnClick;
            _advancedSettingsFoldout = this.Q<Foldout>("advanced-settings");
            _sharedElements = this.Q<VisualElement>("shared-elements");
            _nameElement = this.Q<TextField>("name");
            _pathElement = this.Q<TextField>("path");
            _urlElement = this.Q<TextField>("url");
            _pathElement.RegisterValueChangedCallback(_ =>
            {
                if (_endpoint is null)
                    return;
                _urlElement.value = _endpoint.Url;
            });
            _responseElement = this.Q<ResponseElement>();
            _sharedElementsOriginalIndex = IndexOf(_sharedElements);
        }

        private void ShowAdvancedSettings(bool shouldBeMovedToAdvancedPart)
        {
            _advancedSettingsFoldout.Show(shouldBeMovedToAdvancedPart);
            if (shouldBeMovedToAdvancedPart)
                _advancedSettingsFoldout.Add(_sharedElements);
            else
                Insert(_sharedElementsOriginalIndex, _sharedElements);
        }

        
        public void ShowEndpoint(Endpoint endpoint)
        {
            _endpoint = endpoint;
            var isRequest = !endpoint.IsContainer;
            var serializedObj = new SerializedObject(endpoint);
            _nameElement.label = endpoint.TypeName;
            _requestBodyElement.Q<EnumField>("method").BindProperty(serializedObj);
            _requestBodyElement.Q<PropertyField>("params").BindProperty(serializedObj);
            _requestBodyElement.Init(serializedObj.FindProperty("requestBody"));
            _responseElement.Init(serializedObj.FindProperty("latestResponse"));
            
            ShowAdvancedSettings(isRequest);
            _sharedElements.BindChildrenToProperties(serializedObj);
            this.BindChildrenToProperties(serializedObj);
        }
    }
}