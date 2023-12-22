using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class IndexedButtonListElement<TSelf, TButton, TButtonData> : SelectableListElement<TSelf, TButton, TButtonData> 
        where TButton : IndexedButton<TButton, TButtonData>, new() where TSelf : VisualElement, new() where TButtonData : class
    {
        public IndexedButtonListElement(VisualTreeAsset elementTreeAsset, Color selectColor) : base(elementTreeAsset, selectColor)
        {
        }
        public IndexedButtonListElement() : base()
        {
        }

    }
}