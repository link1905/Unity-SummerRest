using System.IO;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editor.Window.Elements
{
    public class ResponseElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ResponseElement, UxmlTraits>
        {
        }

        public void Init(SerializedProperty property)
        {
            this.Q<PropertyField>("data").BindProperty(property);
            var saveBtn = this.Q<Button>("save-btn");
            saveBtn.clicked += () =>
            {
                property.serializedObject.Update();
                var rawBodyProp = property.FindPropertyRelative("rawBody");
                var filePath = EditorUtility.SaveFilePanel("Save requested file", string.Empty, property.FindPropertyRelative("fileName").stringValue,
                    string.Empty);
                if (string.IsNullOrEmpty(filePath)) 
                    return;
                var bytesProp = property.FindPropertyRelative("rawBytes").FindPropertyRelative("data");
                Debug.Log("Save file into: " + filePath);
                if (bytesProp.arraySize == 0)
                    File.WriteAllText(filePath, rawBodyProp.stringValue);
                else
                {
                    using var nativeBytes = bytesProp.GetByteArray();
                    File.WriteAllBytes(filePath, nativeBytes.ToArray());
                }
            };
        }
    }
}