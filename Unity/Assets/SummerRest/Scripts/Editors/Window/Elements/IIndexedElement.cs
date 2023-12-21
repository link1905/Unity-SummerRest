using System;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public interface IIndexedElement<TElement, TSetupData> where TElement : VisualElement
    {
        event Action<TElement> OnClicked;
        int Index { get; }
        void Init(int index, TSetupData data);
        void Disable();
    }
}