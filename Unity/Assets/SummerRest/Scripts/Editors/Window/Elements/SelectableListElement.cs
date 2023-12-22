using System;
using SummerRest.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class SelectableListElement<TSelf, TElement, TData> : VisualElement 
        where TElement : VisualElement, IIndexedElement<TElement, TData> 
        where TSelf : VisualElement, new()  
        where TData : class
    {
        public event Action<int> OnElementClicked;
        public event Func<TData> OnAdd;
        public event Func<int, bool> OnDeleteElement;
        
        private VisualTreeAsset _elementTreeAsset;
        private string m_Test;
        private Color _selectColor;
        private int? _previousSelect;
        private VisualElement _container;
        private Button _addBtn;
        
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
        public TElement AddChild(TData data)
        {
            var templateContainer = _elementTreeAsset.Instantiate();
            var element = templateContainer.Q<TElement>();
            element.Init(_container.childCount, data);
            element.OnClicked += e =>
            {
                var idx = e.Index;
                if (idx == _previousSelect)
                    return;
                e.style.ReplaceBackgroundColor(_selectColor);
                if (_previousSelect is not null)
                    _container.ElementAt(_previousSelect.Value).Q<TElement>().Disable();
                HandleClick(idx);
            };
            element.OnDeleted += e =>
            {
                var idx = e.Index;
                var del = OnDeleteElement?.Invoke(idx);
                if (del is null || !del.Value)
                    return;
                if (_previousSelect == idx)
                    _previousSelect = null;
                _container.RemoveAt(idx);
                for (var i = idx; i < _container.childCount; i++)
                    _container[i].Q<TElement>().Index = i;
            };
            _container.Add(element);
            return element;
        }

        private void HandleClick(int idx)
        {
            _previousSelect = idx;
            OnElementClicked?.Invoke(idx);
        }

        public void Init(VisualTreeAsset elementTreeAsset, Color selectColor)
        {
            _elementTreeAsset = elementTreeAsset;
            _selectColor = selectColor;
            _container = this.Q<VisualElement>("container");
            var addBtn = this.Q<Button>("add-btn");
            addBtn.clicked += () =>
            {
                var newElement = OnAdd?.Invoke();
                if (newElement != null)
                    AddChild(newElement);
            };
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