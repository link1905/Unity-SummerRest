using System;
using SummerRest.Editor.DataStructures;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class MultipartFormRow : TextOrCustomData<MultipartFormRowType>
    {
        [SerializeField] private string key;
        [SerializeField] private FileReference file;
        public KeyValue Pair
        {
            get
            {
                if (type == MultipartFormRowType.PlainText)
                    return new KeyValue(key, text);
                return new KeyValue(key, file.FilePath);
            }
        }
        public IMultipartFormSection FormSection
        {
            get
            {
                if (type == MultipartFormRowType.PlainText)
                    return new MultipartFormDataSection(key, text);
                if (!file.Valid)
                    return null;
                return new MultipartFormFileSection(key, file.Data, file.FileName, null);
            }
        }
    }
}