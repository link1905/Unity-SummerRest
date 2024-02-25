using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SummerRest.Editor.Utilities
{
    public static class EditorAssetUtilities
    {
        public static T CreateAndSaveObject<T>(string name, string path) where T : ScriptableObject
        {
            var obj = ScriptableObject.CreateInstance<T>();
            obj.name = name;
            var assetPath = Path.Combine(path, $"{name}.asset");
            AssetDatabase.CreateAsset(obj, assetPath);
            AssetDatabase.SaveAssets();
            return obj;
        }

        public static T LoadOrCreate<T>(string path, Func<T> onCreate) where T : Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset is not null) 
                return asset;
            asset = onCreate.Invoke();
            CreateFileIfNotExist(path);
            return asset;
        }

        public static void CreateFileIfNotExist(string path)
        {
            if (!File.Exists(path))
                File.Create(path).Dispose();
        }
        public static void LoadOrCreateTextFile(string path, string content)
        {
            if (!File.Exists(path))
                File.Create(path).Dispose();
            WriteTextContent(path, content);
        }
        public static void WriteTextContent(string filePath, string content)
        {
            File.WriteAllText(filePath, string.Empty);
            File.WriteAllText(filePath, content);
        }

        public static void CreateFolderIfNotExists(string parent, string name)
        {
            if (!AssetDatabase.IsValidFolder(Path.Combine(parent, name)))
                AssetDatabase.CreateFolder(parent, name);
        }

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

        public static void MakeDirty(this Object o)
        {
            EditorUtility.SetDirty(o);
        }
        
        public static bool AskToRemoveAsset<T>(this T obj, Action<T> deleteAction, AskToRemoveMessage message = null) where T : Object
        {
            if (message is null || message.ShowDialog())
            {
                deleteAction.Invoke(obj);
                return true;
            }
            return false;
        }
        public static bool RemoveAsset<T>(this T obj) where T : Object
        {
            return AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(obj));
        }

        public static string GetAssetPath(this Object obj) => 
            AssetDatabase.GetAssetPath(obj);
        public static string GetAssetFolder(this Object obj) => 
            Path.GetDirectoryName(AssetDatabase.GetAssetPath(obj));
    }
}