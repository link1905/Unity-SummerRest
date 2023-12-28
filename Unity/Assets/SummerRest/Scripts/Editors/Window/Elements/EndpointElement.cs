using System;
using SummerRest.Models;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class EndpointElement : VisualElement
    {
        public event Action<Endpoint> OnRequest;
        private Button _requestBtn;
        private Endpoint _endpoint;
        private Foldout _advancedSettingsFoldout;
        private VisualElement _sharedElements;
        private TextField _nameElement;
        private TextField _pathElement;
        private RequestBodyElement _requestBodyElement; 
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
            _requestBtn = this.Q<Button>("request-btn");
            _requestBtn.clicked += OnClick;
            _advancedSettingsFoldout = this.Q<Foldout>("advanced-settings");
            _sharedElements = this.Q<VisualElement>("shared-elements");
            _nameElement = this.Q<TextField>("name");
            _pathElement = this.Q<TextField>("path");
            _requestBodyElement = this.Q<RequestBodyElement>();
            _responseElement = this.Q<ResponseElement>();
        }

        private void ShowAdvancedSettings(bool shouldBeMovedToAdvancedPart)
        {
            _advancedSettingsFoldout.Show(shouldBeMovedToAdvancedPart);
            if (shouldBeMovedToAdvancedPart)
                _advancedSettingsFoldout.Add(_sharedElements);
            else
                Insert(0, _sharedElements);
        }

        
        public void Init(Endpoint endpoint)
        {
            _endpoint = endpoint;
            var isRequest = !endpoint.IsContainer;
            var serializedObj = new SerializedObject(endpoint);
            _nameElement.label = endpoint.TypeName;
            _pathElement.label = endpoint.ParentPath;
            _requestBodyElement.Init(serializedObj.FindProperty("requestBody"));
            _responseElement.Init(serializedObj.FindProperty("latestResponse"));
            _requestBtn.Show(isRequest);
            ShowAdvancedSettings(isRequest);
            _sharedElements.BindChildrenToProperties(serializedObj);
            this.BindChildrenToProperties(serializedObj);
        }
    }
}