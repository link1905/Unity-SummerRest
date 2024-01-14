using SummerRest.Editor.Models;
using UnityEditor;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(RequestBody))]
    internal class RequestBodyDrawer : TextOrCustomDataDrawer
    {
        public override string RelativeFromTemplateAssetPath => "Properties/request-body.uxml";
    }
}