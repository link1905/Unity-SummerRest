using System;
using SummerRest.Editor.Models;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(AuthContainer))]
    internal class AuthContainerDrawer : TextOrCustomDataDrawer
    {
        protected override string RelativeFromTemplateAssetPath => "Properties/auth-container.uxml";
        public override Enum DefaultEnum => AuthContainer.Type.PlainText;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var foldout = new Foldout
            {
                value = property.isExpanded
            };
            var key = property.FindPropertyRelative("key");
            foldout.text = key.stringValue;
            foldout.TrackPropertyValue(key, s => foldout.text = s.stringValue);
            var baseTree = base.CreatePropertyGUI(property);
            baseTree.Q<EnumField>("type").SetEnabled(false);
            foldout.contentContainer.Add(baseTree);
            return foldout;
        }
    }
}