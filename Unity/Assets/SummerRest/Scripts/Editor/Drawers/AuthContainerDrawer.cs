using SummerRest.Editor.Models;
using UnityEditor;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(AuthContainer))]
    internal class AuthContainerDrawer : TextOrCustomDataDrawer
    {
        public override string AssetPath => "Assets/SummerRest/Editors/Templates/Properties/auth-container.uxml";
    }
}