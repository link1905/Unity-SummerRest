using SummerRest.Editor.Models;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    public class DomainListElement : SelectableListElement<DomainListElement, DomainItemElement, Domain>
    {
        public new class UxmlFactory : UxmlFactory<DomainListElement, UxmlTraits>
        {
        }
    }
}