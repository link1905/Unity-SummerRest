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
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            if (File.Exists(filePath))
                return;
            filePath = string.Empty;
        }
    }
}