using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class IndexedButtonListElement : SelectableListElement<IndexedButtonListElement, IndexedButton, string>
    {
        public IndexedButtonListElement(VisualTreeAsset elementTreeAsset, Color selectColor) : base(elementTreeAsset, selectColor)
        {
        }
        public IndexedButtonListElement() : base()
        {
        }
        public new class UxmlFactory : UxmlFactory<IndexedButtonListElement, UxmlTraits>
        {
        }
    }
}