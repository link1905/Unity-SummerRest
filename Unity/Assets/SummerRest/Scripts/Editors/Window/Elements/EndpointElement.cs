using System;
using SummerRest.Models;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class EndpointElement : VisualElement
    {
        public event Action<Endpoint> OnRequest;
        private Button _requestBtn;
        private Endpoint _endpoint;
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
        }

        public void Init(Endpoint endpoint)
        {
            _endpoint = endpoint;
            var serializedObj = new SerializedObject(endpoint);
            this.Q<TextField>("name").label = endpoint.ParentPath;
            var bodyProperty = serializedObj.FindProperty("requestBody");
            this.Q<RequestBodyElement>().Init(bodyProperty);
            var latestResponse = serializedObj.FindProperty("latestResponse");
            _requestBtn.style.Show(latestResponse != null);
            this.Q<ResponseElement>().Init(latestResponse);
            this.BindChildrenToProperties(serializedObj);
        }
    }
}