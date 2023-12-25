using System;
using SummerRest.Models;
using SummerRest.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class DomainElement : VisualElement, IIndexedElement<DomainElement, Domain>
    {
        public event Action<DomainElement> OnClicked;
        public event Action<DomainElement> OnDeleted;
        private Color _originalColor;
        public int Index { get; set; }
        public void Init(int index, Domain data)
        {
            Index = index;
            _originalColor = style.backgroundColor.value;
            var mainBtn = this.Q<Button>(name: "main-btn");
            mainBtn.BindProperty(new SerializedObject(data));
            var delBtn = this.Q<Button>(name: "del-btn");
            mainBtn.clicked += () =>
            {
                OnClicked?.Invoke(this);
            };
            delBtn.clicked += () =>
            {
                OnDeleted?.Invoke(this);
            };
        }

        public void Disable()
        {
            style.ReplaceBackgroundColor(_originalColor);
        }
        public new class UxmlFactory : UxmlFactory<DomainElement, UxmlTraits>
        {
        }
    }
}