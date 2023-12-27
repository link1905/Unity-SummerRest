using System;
using SummerRest.Models;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public enum ElementAddAction
    {
        Service, Request
    }

    public class EndpointTreeElement : VisualElement
    {
        private ToolbarMenu _actionMenu;
        private Label _nameElement;
        private Button _delBtn;
        private Endpoint _endpoint;
        public event Action<ElementAddAction, Endpoint> OnAddChild; 
        public event Action<Endpoint> OnDelete; 
        public new class UxmlFactory : UxmlFactory<EndpointTreeElement, UxmlTraits>
        {
        }
        public void Init(Endpoint endpoint)
        {
            _actionMenu = this.Q<ToolbarMenu>("action-menu");
            _nameElement = this.Q<Label>("name");
            
            _endpoint = endpoint;
            var serializedObj = new SerializedObject(endpoint);
            _nameElement.BindProperty(serializedObj);
                
            this.AddManipulator(new ContextualMenuManipulator(OnContextClick));
            _actionMenu.style.Show(endpoint.IsContainer);
            _actionMenu.menu.ClearItems();
            _actionMenu.menu.AppendAction(ElementAddAction.Service.ToString(), ClickCreate);
            _actionMenu.menu.AppendAction(ElementAddAction.Request.ToString(), ClickCreate);
            if (!endpoint.IsContainer)
                return;
        }
        private void OnContextClick(ContextualMenuPopulateEvent evt)
        {
            // Populate the context menu with options
            evt.menu.AppendAction("Delete", _ => OnDelete?.Invoke(_endpoint));
            // Add more options as needed
        }
        private void ClickCreate(DropdownMenuAction action)
        {
            OnAddChild?.Invoke(Enum.Parse<ElementAddAction>(action.name), _endpoint);
        }
        public EndpointTreeElement()
        {
        }
    }
}