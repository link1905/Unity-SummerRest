using System.IO;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editors.Utilities
{
    public static class SummerRestAssetUtilities
    {
        public static T CreateAndSaveObject<T>(string name, string path) where T : ScriptableObject
        {
            var obj = ScriptableObject.CreateInstance<T>();
            obj.name = name;
            AssetDatabase.CreateAsset(obj, Path.Combine(path, $"{name}.asset"));
            AssetDatabase.SaveAssets();
            return obj;
        }

        public static void CreateFolderIfNotExists(string parent, string name)
        {
            if (!AssetDatabase.IsValidFolder(Path.Combine(parent, name)))
                AssetDatabase.CreateFolder(parent, name);
        }

        //public static bool NotStableObject(this Object self) => self is null || self.GetInstanceID() <= 0;
        public static T CreateAndSaveObject<T>(string path) where T : ScriptableObject
        {
            var randName = GUID.Generate().ToString();
            return CreateAndSaveObject<T>(randName, path);
        }
        
        public class AskToRemoveMessage
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public string Ok { get; set; }
            public string Cancel { get; set; } = nameof(Cancel);

            public static readonly AskToRemoveMessage RemoveDomain = new()
            {
                Title = "Remove domain",
                Message = "Do you really want to remove the domain including its children?",
                Ok = "Yes, please remove it!",
            };
            public static readonly AskToRemoveMessage RemoveService = new()
            {
                Title = "Remove service",
                Message = "Do you really want to remove the service including its children?",
                Ok = "Yes, please remove it!",
            };


            public bool ShowDialog()
            {
                return EditorUtility.DisplayDialog(Title, Message, Ok, Cancel);
            }
        }

        public static bool RemoveAsset(this Object obj, AskToRemoveMessage message = null)
        {
            if (message is not null && message.ShowDialog())
                return AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(obj));
            return false;
        }

        public static string GetAssetFolder(this Object obj) => 
            Path.GetDirectoryName(AssetDatabase.GetAssetPath(obj));

        public static string GetAssetFolder(this Object obj, string combine) =>
            System.IO.Path.Combine(obj.GetAssetFolder(), combine);
    }
}