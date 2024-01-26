﻿using System;
using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    public enum ElementAddAction
    {
        Service, Request
    }

    /// <summary>
    /// Represents an endpoint on a <see cref="TreeView"/>
    /// </summary>
    public class EndpointTreeElement : VisualElement
    {
        private ToolbarMenu _actionMenu;
        private Label _nameElement;
        private Label _path;
        private Label _method;
        private Button _delBtn;
        private Endpoint _endpoint;
        public event Action<ElementAddAction, Endpoint> OnAddChild; 
        public event Action<Endpoint> OnDelete; 
        public new class UxmlFactory : UxmlFactory<EndpointTreeElement, UxmlTraits>
        {
        }
        public void Init(Endpoint endpoint)
        {
            var isContainer = endpoint.IsContainer;
            _actionMenu = this.Q<ToolbarMenu>("action-menu");
            _nameElement = this.Q<Label>("name");
            _method = this.Q<Label>("method");
            _path = this.Q<Label>("path");
            
            _endpoint = endpoint;
            var serializedObj = new SerializedObject(endpoint);
            _nameElement.BindProperty(serializedObj);
            _path.BindWithCallback<Label, string>(serializedObj, SetPath);
            this.AddManipulator(new ContextualMenuManipulator(e => OnContextClick(isContainer, e.menu)));
            _actionMenu.Show(isContainer);
            _method.Show(!isContainer);
            if (!isContainer)
                _method.BindWithCallback<Label, string>(serializedObj, SetMethod);
        }

        private void SetMethod(string val)
        {
            _method.SetTextValueWithoutNotify($"({val})");
        }
        private void SetPath(string value)
        {
            _path.SetTextValueWithoutNotify($"(/{value})");
        }
        /// <summary>
        /// Show actions for adding children to this element
        /// </summary>
        /// <param name="container"></param>
        /// <param name="menu"></param>
        private void OnContextClick(bool container, DropdownMenu menu)
        {
            menu.ClearItems();
            if (container) //Single action => requests have no child, no need to show adding actions 
            {
                menu.AppendAction(ElementAddAction.Service.ToString(), ClickCreate);
                menu.AppendAction(ElementAddAction.Request.ToString(), ClickCreate); 
            }
            menu.AppendAction("Delete", _ => OnDelete?.Invoke(_endpoint));
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