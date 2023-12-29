using System;
using SummerRest.Models;
using SummerRest.Scripts.Utilities;
using SummerRest.Scripts.Utilities.Common;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class DomainItemElement : VisualElement, IIndexedElement<DomainItemElement, Domain>
    {
        public event Action<DomainItemElement> OnClicked;
        public event Action<DomainItemElement> OnDeleted;
        private Color _originalColor;
        private Button _mainBtn;
        public IStyle Style => _mainBtn.style;
        public int Index { get; set; }
        public void Init(int index, Domain data)
        {
            var serializedObject = new SerializedObject(data);
            Index = index;
            _originalColor = style.backgroundColor.value;
            _mainBtn = this.Q<Button>(name: "main-btn");
            _mainBtn.BindWithCallback<Button, string>(serializedObject, SetEndpointName);
            _mainBtn.clicked += () =>
            {
                OnClicked?.Invoke(this);
            };
            this.AddManipulator(new ContextualMenuManipulator(OnContextClick));
        }
        private void SetEndpointName(string value)
        {
            var show = string.IsNullOrEmpty(value) ? "(Anonymous)" : value;
            _mainBtn.SetTextValueWithoutNotify(show);
        }

        public void Enable(Color highlight)
        {
            _mainBtn.style.ReplaceBackgroundColor(highlight);
        }

        private void OnContextClick(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Delete", _ => OnDeleted?.Invoke(this));
        }
        public void Disable()
        {
            _mainBtn.style.ReplaceBackgroundColor(_originalColor);
        }
        public new class UxmlFactory : UxmlFactory<DomainItemElement, UxmlTraits>
        {
        }
    }
}