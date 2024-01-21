using SummerRest.Editor.Models;
using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(AuthContainer))]
    internal class AuthContainerDrawer : TextOrCustomDataDrawer
    {
        protected override string RelativeFromTemplateAssetPath => "Properties/auth-container.uxml";
    }
}