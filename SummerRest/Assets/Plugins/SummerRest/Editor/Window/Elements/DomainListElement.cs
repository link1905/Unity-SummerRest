using SummerRest.Editor.Models;
using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    /// <summary>
    /// Show a list of <see cref="DomainItemElement"/> on the top of <see cref="SummerRestConfigurationElement"/>
    /// </summary>
    public class DomainListElement : SelectableListElement<DomainListElement, DomainItemElement, Domain>
    {
        public new class UxmlFactory : UxmlFactory<DomainListElement, UxmlTraits>
        {
        }
    }
}