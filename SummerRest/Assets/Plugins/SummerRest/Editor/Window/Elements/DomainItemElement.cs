using System;
using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    /// <summary>
    /// A domain element shown inside <see cref="DomainListElement"/>
    /// </summary>
    public class DomainItemElement : VisualElement, IIndexedElement<DomainItemElement, Domain>
    {
        public event Action<DomainItemElement> OnClicked;
        public event Action<DomainItemElement> OnDeleted;
        private Color _originalColor;
        private Button _mainBtn;
        public IStyle Style => _mainBtn.style;
        public Domain Data { get; private set; }
        public int Index { get; set; }
        public void Init(int index, Domain data)
        {
            Data = data;
            var serializedObject = new SerializedObject(data);
            Index = index;
            _originalColor = style.backgroundColor.value;
            _mainBtn = this.Q<Button>(name: "main-btn");
            _mainBtn.CallThenTrackPropertyValue(serializedObject, s => SetEndpointName(s.stringValue));
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