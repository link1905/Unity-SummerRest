using SummerRest.Editor.Models;
using SummerRest.Editor.Window.Elements;
using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ResponseBody))]
    internal class ResponseBodyDrawer : UIToolkitDrawer
    {
        public override string RelativeFromTemplateAssetPath => "Properties/file-response-element.uxml";
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = Tree;
            tree.Q<ResponseElement>().Init(property);
            return tree;
        }

    }
}