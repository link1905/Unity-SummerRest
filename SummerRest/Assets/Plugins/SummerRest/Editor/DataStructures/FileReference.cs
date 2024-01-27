using System;
using System.IO;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    [Serializable]
    public class FileReference : ISerializationCallbackReceiver
    {
        [SerializeField] private string filePath;
        public string FilePath => filePath;
        public string FileName => Path.GetFileName(filePath);
        public byte[] Data => Valid ? File.ReadAllBytes(filePath) : null;
        public bool Valid => File.Exists(filePath);
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            if (Valid)
                return;
            filePath = string.Empty;
        }
    }
}