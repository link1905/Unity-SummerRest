using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using Unity.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(BytesBody))]
    internal class BytesBodyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var image = new Image
            {
                scaleMode = ScaleMode.ScaleToFit
            };
            image.CallThenTrackPropertyValue(property, p =>
            {
                p.serializedObject.Update();
                SetImage(p, image);
            });
            return image;
        }
        
        private void SetImage(SerializedProperty property, Image image)
        {
            var bytesProp = property.FindPropertyRelative("data");
            var isImageProp = property.FindPropertyRelative("isImage");
            // Not an image => show nothing
            // This method is planned to show more types of bytes values like sound,video,...
            if (bytesProp.arraySize == 0 || !isImageProp.boolValue)
            {
                image.Show(false);
                return;
            }
            using var nativeBytes = bytesProp.GetArrayValue();
            var text = image.image as Texture2D ?? new Texture2D(1, 1);
            text.LoadImage(nativeBytes.ToArray());
            text.Apply(true);
            image.image = text;
            image.Show(true);
        }
    }
}