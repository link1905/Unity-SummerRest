using System;
using SummerRest.Editor.DataStructures;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class MultipartFormRow : TextOrCustomData<MultipartFormRowType>
    {
        [SerializeField] private string key;
        [SerializeField] private FileReference file;

        public KeyValue? SerializedPair
        {
            get
            {
                if (type == MultipartFormRowType.PlainText)
                    return new KeyValue(key, text);
                return null;
            }
        }
    }
}