using SummerRest.Editor.Models;
using UnityEditor;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(RequestBody))]
    internal class RequestBodyDrawer : TextOrCustomDataDrawer
    {
        public override string AssetPath => "Assets/SummerRest/Editors/Templates/Properties/request-body.uxml";
    }
}