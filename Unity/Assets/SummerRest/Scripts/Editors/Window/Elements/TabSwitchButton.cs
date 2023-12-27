using System;
using SummerRest.Scripts.Utilities;
using SummerRest.Scripts.Utilities.Editor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class TabSwitchButton : ToolbarButton, IIndexedElement<TabSwitchButton, VisualElement>
    {
        public event Action<TabSwitchButton> OnClicked;
        public event Action<TabSwitchButton> OnDeleted;
        private VisualElement _associatedContainer;
        private Color _original;
        public int Index { get; set; }
        public void Init(int index, VisualElement data)
        {
            _associatedContainer = data;
            _original = style.backgroundColor.value;
            clicked += () =>
            {
                OnClicked?.Invoke(this);
            };
        }
        public void Enable(Color highlight)
        {
            style.ReplaceBackgroundColor(highlight);
            _associatedContainer.style.Show(true);
        }
        public void Disable()
        {
            _associatedContainer.style.Show(false);
            style.ReplaceBackgroundColor(_original);
        }
        public new class UxmlFactory : UxmlFactory<TabSwitchButton, UxmlTraits>
        {
        }
        public TabSwitchButton()
        {
            
        }
    }
}