using System;
using SummerRest.Editors.Utilities;
using SummerRest.Models;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public enum ElementAddAction
    {
        Service, Request
    }
    public class EndpointElement : VisualElement
    {

        private readonly ToolbarMenu _actionMenu;
        private readonly TextField _nameElement;
        private readonly Button _delBtn;
        private Endpoint _endpoint;
        public event Action<ElementAddAction, Endpoint> OnAddChild; 
        public event Action<Endpoint> OnDelete; 
        public new class UxmlFactory : UxmlFactory<EndpointElement, UxmlTraits>
        {
        }
        public void Init(Endpoint endpoint)
        {
            _endpoint = endpoint;
            var serializedObj = new SerializedObject(endpoint);
            _nameElement.BindProperty(serializedObj);
            _delBtn.clicked += () => OnDelete?.Invoke(_endpoint);
                
            _actionMenu.style.Show(endpoint.IsContainer);
            _actionMenu.menu.ClearItems();
            _actionMenu.menu.AppendAction(ElementAddAction.Service.ToString(), ClickCreate);
            _actionMenu.menu.AppendAction(ElementAddAction.Request.ToString(), ClickCreate);
            if (!endpoint.IsContainer)
                return;
        }
        private void ClickCreate(DropdownMenuAction action)
        {
            OnAddChild?.Invoke(Enum.Parse<ElementAddAction>(action.name), _endpoint);
        }
        public EndpointElement()
        {
            _actionMenu = this.Q<ToolbarMenu>("action-menu");
            _nameElement = this.Q<TextField>("name");
            _delBtn = this.Q<Button>("del-btn");
        }
    }
}