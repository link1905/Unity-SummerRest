using SummerRest.Editor.Configurations;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    public class AuthConfiguresElement : VisualElement
    {
        private PropertyField _auths;
        private PropertyField _defaultRepository;
        public new class UxmlFactory : UxmlFactory<AuthConfiguresElement, UxmlTraits>
        {
        }

        public void Show(AuthenticateConfiguration configuration)
        {
            this.BindChildrenToProperties(new SerializedObject(configuration));
        }
    }
}