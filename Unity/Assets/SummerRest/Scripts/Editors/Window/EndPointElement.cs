using UnityEngine.UIElements;

namespace SummerRest.Editors.Window
{
    public class EndPointElement : VisualElement
    {
        // Expose the custom control to UXML and UI Builder.
        public new class UxmlFactory : UxmlFactory<EndPointElement, UxmlTraits> {}
        private Label Name => this.Q<Label>("name");
        private Label Type => this.Q<Label>("type");
        public void Init(string name, string type)
        {
            Name.text = name;
            Type.text = type;
        }
        // Custom controls need a default constructor. 
        public EndPointElement() {}
    }
}