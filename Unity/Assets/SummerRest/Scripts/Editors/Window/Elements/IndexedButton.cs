using System;
using SummerRest.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class IndexedButton : Button, IIndexedElement<IndexedButton, string>
    {
        public int Index { get; private set; }
        public event Action<IndexedButton> OnClicked;
        private Color _originalColor;
        public void Init(int index, string text)
        {
            Index = index;
            base.text = text;
            _originalColor = style.backgroundColor.value;
            clicked += () =>
            {
                OnClicked?.Invoke(this);
            };
        }
        public void Disable()
        {
            style.ReplaceBackgroundColor(_originalColor);
        }
        public new class UxmlFactory : UxmlFactory<IndexedButton, UxmlTraits>
        {
        }
        public IndexedButton()
        {
            
        }
    }
}