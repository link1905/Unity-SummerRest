using System;
using SummerRest.Editor.Models;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(MultipartFormRow))]
    internal class MultipartFormRowDrawer : TextOrCustomDataDrawer
    {
        protected override string RelativeFromTemplateAssetPath => "Properties/multipart-form-row.uxml";
        public override Enum DefaultEnum => MultipartFormRowType.PlainText;
        protected override VisualElement[] GetShownElements(VisualElement tree)
        {
            return new VisualElement[]
            {
                tree.Q<TextField>("text"),
                tree.Q<PropertyField>("file")
            };
        }
    }
}