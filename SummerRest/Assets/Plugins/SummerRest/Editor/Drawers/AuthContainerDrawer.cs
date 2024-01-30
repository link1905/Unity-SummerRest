using System;
using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(AuthContainer))]
    internal class AuthContainerDrawer : TextOrCustomDataDrawer
    {
        protected override string RelativeFromTemplateAssetPath => "Properties/auth-container.uxml";
        public override Enum DefaultEnum => AuthContainer.AuthType.PlainText;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var foldout = new Foldout
            {
                value = property.isExpanded
            };
            var key = property.FindPropertyRelative("key");
            foldout.CallThenTrackPropertyValue(key, s =>
            {
                try
                {
                    foldout.text = s.stringValue;
                }
                catch (Exception)
                {
                    foldout.UnBindAllChildren();
                    // After deleting an auth container => its serialized property is disposed
                    // But we have no way to detect it
                }
            });
            var baseTree = base.CreatePropertyGUI(property);
            baseTree.Q<EnumField>("type").SetEnabled(false);
            foldout.contentContainer.Add(baseTree);
            return foldout;
        }
    }
}