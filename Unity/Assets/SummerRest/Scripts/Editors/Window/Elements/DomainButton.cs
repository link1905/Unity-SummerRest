using SummerRest.Models;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class DomainButton : IndexedButton<DomainButton, Domain>
    {
        public override void Init(int index, Domain data)
        {
            base.Init(index, data.Name);
        }
        public new class UxmlFactory : UxmlFactory<DomainButton, UxmlTraits>
        {
        }
    }
}