using System;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    internal abstract class TextOrCustomDataDrawer : UIToolkitDrawer
    {
        protected override string RelativeFromTemplateAssetPath => "Properties/text-or-custom.uxml";
        public abstract Enum DefaultEnum { get; }
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = Tree;
            var typeElement = tree.Q<EnumField>("type");
            var typeProp = property.FindPropertyRelative("type");
            typeElement.Init(DefaultEnum);
            typeElement.RegisterValueChangedCallback(e =>
            {
                var newVal = e.newValue;
                if (newVal is null)
                    return;
                BindValueElement((int)Convert.ChangeType(newVal, newVal.GetTypeCode()), tree);
            });
            BindValueElement(typeProp.enumValueIndex, tree);
            return tree;
        }

        protected virtual VisualElement[] GetShownElements(VisualElement tree)
        {
            return new VisualElement[]
            {
                tree.Q<TextField>("text"),
                tree.Q<PropertyField>("data")
            };
        }

        protected virtual void BindValueElement(int value, VisualElement tree)
        {
            var elements = GetShownElements(tree);
            foreach (var element in elements)
                element.Show(false);
            elements[value].Show(true);
        }
    }
}