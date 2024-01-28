using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    /// <summary>
    /// A custom listview showing the elements horizontally <bt/>
    /// It also highlight current selected element
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class SelectableListElement<TSelf, TElement, TData> : VisualElement 
        where TElement : VisualElement, IIndexedElement<TElement, TData> 
        where TSelf : VisualElement, new()  
        where TData : class
    {
        public event Action<int> OnElementClicked;
        // public event Func<TData> OnAdd;
        public event Func<int, bool, bool> OnDeleteElement;
        
        private VisualTreeAsset _elementTreeAsset;
        private Color _selectColor;
        // Current selected element's index
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

        /// <summary>
        /// Add a child to the list
        /// </summary>
        /// <param name="element"></param>
        /// <param name="data">The data to setup the child</param>
        /// <param name="alsoSelect">Also selected the child after finishing</param>
        /// <returns></returns>
        public virtual TElement AddChild(TElement element, TData data, bool alsoSelect)
        {
            element.Init(_container.childCount, data);
            element.OnClicked += HandleClick;
            element.OnDeleted += e =>
            {
                var idx = e.Index;
                var del = OnDeleteElement?.Invoke(idx, _previousSelect == idx);
                if (del is null || !del.Value)
                    return;
                if (_previousSelect == idx)
                    _previousSelect = null;
                _container.RemoveAt(idx);
                for (var i = idx; i < _container.childCount; i++)
                    _container[i].Q<TElement>().Index = i;
            };
            _container.Add(element);
            if (alsoSelect)
                HandleClick(element);
            return element;
        }

        public void DeleteChild(TData data)
        {
            var idx = -1;
            for (var i = 0; i < _container.childCount; i++)
            {
                var element = _container[i].Q<TElement>();
                if (element is null)
                    continue;
                if (element.Data == data)
                    idx = i;
            }
            if (idx == -1)
                return;
            DeleteChild(idx);
        }
        public void DeleteChild(int idx)
        {
            var del = OnDeleteElement?.Invoke(idx, _previousSelect == idx);
            if (del is null || !del.Value)
                return;
            if (_previousSelect == idx)
                _previousSelect = null;
            _container.RemoveAt(idx);
            for (var i = idx; i < _container.childCount; i++)
                _container[i].Q<TElement>().Index = i;
        }

        public virtual TElement AddChild(TData data, bool alsoSelect)
        {
            var templateContainer = _elementTreeAsset.Instantiate();
            var element = templateContainer.Q<TElement>();
            return AddChild(element, data, alsoSelect);
        }

        protected virtual void HandleClick(TElement element)
        {
            var idx = element.Index;
            if (idx == _previousSelect)
                return;
            // Highlight new selection
            element.Enable(_selectColor);
            // Disable the existing one
            if (_previousSelect is not null)
                _container.ElementAt(_previousSelect.Value).Q<TElement>().Disable();
            _previousSelect = idx;
            OnElementClicked?.Invoke(idx);
        }

        public void Init(VisualTreeAsset elementTreeAsset, Color selectColor)
        {
            _elementTreeAsset = elementTreeAsset;
            Init(selectColor);
        }
        public virtual void Init(Color selectColor)
        {
            _selectColor = selectColor;
            _container = this.Q<VisualElement>("container");
            _previousSelect = null;
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