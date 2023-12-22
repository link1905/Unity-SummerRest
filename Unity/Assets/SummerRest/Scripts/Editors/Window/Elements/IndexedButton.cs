using System;
using SummerRest.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public abstract class IndexedButton<TSelf, TData> : Button, IIndexedElement<TSelf, TData> where TSelf : IndexedButton<TSelf, TData>, new()
    {
        public event Action<TSelf> OnDeleted;
        public int Index { get; set; }
        public event Action<TSelf> OnClicked;
        private Color _originalColor;
        public void Init(int index, string text)
        {
            Index = index;
            base.text = text;
            _originalColor = style.backgroundColor.value;
            clicked += () =>
            {
                OnClicked?.Invoke((TSelf)this);
            };
            this.Q<Button>("del-btn").clicked += () =>
            {
                OnDeleted?.Invoke((TSelf)this);
            };
        }
  
        public abstract void Init(int index, TData data);
        public void Disable()
        {
            style.ReplaceBackgroundColor(_originalColor);
        }
        public IndexedButton()
        {
            
        }
    }
}