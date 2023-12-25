using SummerRest.Models;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class DomainListElement : SelectableListElement<DomainListElement, DomainElement, Domain>
    {
        public new class UxmlFactory : UxmlFactory<DomainListElement, UxmlTraits>
        {
        }
    }
}