using System;
using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    public class DomainElement : VisualElement, IIndexedElement<DomainElement, Domain>
    {
        public event Action<DomainElement> OnClicked;
        public event Action<DomainElement> OnDeleted;
        private Color _originalColor;
        private Button _mainBtn;
        public IStyle Style => _mainBtn.style;
        public Domain Data { get; private set; }
        public int Index { get; set; }
        public void Init(int index, Domain data)
        {
            Data = data;
            Index = index;
            _originalColor = style.backgroundColor.value;
            _mainBtn = this.Q<Button>(name: "main-btn");
            _mainBtn.BindProperty(new SerializedObject(data));
            _mainBtn.clicked += () =>
            {
                OnClicked?.Invoke(this);
            };
            this.AddManipulator(new ContextualMenuManipulator(OnContextClick));
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
        public new class UxmlFactory : UxmlFactory<DomainElement, UxmlTraits>
        {
        }
    }
}