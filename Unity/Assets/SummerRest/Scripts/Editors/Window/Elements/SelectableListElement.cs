using System;
using SummerRest.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class SelectableListElement<TSelf, TElement, TData> : VisualElement 
        where TElement : VisualElement, IIndexedElement<TElement, TData> 
        where TSelf : VisualElement, new()
    {
        public event Action<int> OnElementClicked;
        private VisualTreeAsset _elementTreeAsset;
        private string m_Test;
        private Color _selectColor;
        private int? _previousSelect;
        public new class UxmlFactory : UxmlFactory<TSelf, UxmlTraits>
        {
        }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlColorAttributeDescription m_selectColor;
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var c = Color.white;
                if (m_selectColor.TryGetValueFromBag(bag, cc, ref c))
                    ((SelectableListElement<TSelf, TElement, TData>)ve)._selectColor = c;
            }

            public UxmlTraits() : base()
            {
                var selectColorDes = new UxmlColorAttributeDescription
                {
                    name = "Select color"
                };
                this.m_selectColor = selectColorDes;
            }
        }
        public void AddChild(TData data)
        {
            var templateContainer = _elementTreeAsset.Instantiate();
            var element = templateContainer.Q<TElement>();
            element.Init(childCount, data);
            element.OnClicked += e =>
            {
                var idx = e.Index;
                if (idx == _previousSelect)
                    return;
                e.style.ReplaceBackgroundColor(_selectColor);
                if (_previousSelect is not null)
                    ElementAt(_previousSelect.Value).Q<TElement>().Disable();
                _previousSelect = idx;
                OnElementClicked?.Invoke(idx);
            };
            Add(element);
        }

        public void Init(VisualTreeAsset elementTreeAsset, Color selectColor)
        {
            _elementTreeAsset = elementTreeAsset;
            _selectColor = selectColor;
        }
        public SelectableListElement(VisualTreeAsset elementTreeAsset, Color selectColor)
        {
            Init(elementTreeAsset, selectColor);
        }
        public SelectableListElement()
        {
        }
    }
}