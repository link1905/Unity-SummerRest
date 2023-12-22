using SummerRest.Models;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class DomainListElement : IndexedButtonListElement<DomainListElement, DomainButton, Domain>
    {
        public new class UxmlFactory : UxmlFactory<DomainListElement, UxmlTraits>
        {
        }
    }
}