using System.IO;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(FileReference))]
    internal class FileReferenceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var pathBtn = new Button();
            var pathProp = property.FindPropertyRelative("filePath");
            pathBtn.CallThenTrackPropertyValue(pathProp, s =>
            {
                pathBtn.text = string.IsNullOrEmpty(s.stringValue) ? "Select file" : Path.GetFileName(s.stringValue);
            });
            pathBtn.clicked += () =>
            {
                var path = EditorUtility.OpenFilePanel("Select file", string.Empty, string.Empty);
                pathProp.serializedObject.Update();
                pathProp.stringValue = File.Exists(path) ? path : string.Empty;
                pathProp.serializedObject.ApplyModifiedProperties();
            };
            return pathBtn;
        }
    }
}