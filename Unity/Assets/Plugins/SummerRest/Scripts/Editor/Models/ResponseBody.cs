using System;
using SummerRest.Editor.Attributes;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class ResponseBody
    {
        [SerializeField] private string mediaType;
        [SerializeField] private string fileName;
        [SerializeField] private BytesBody rawBytes;
        [SerializeField, TextMultiline] private string rawBody;
        public string RawBody
        {
            get => rawBody;
            set => rawBody = value;
        }
        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }
        public string MediaType
        {
            get => mediaType;
            set => mediaType = value;
        }
        public BytesBody RawBytes
        {
            get => rawBytes;
            set => rawBytes = value;
        }
        public void Clear()
        {
            fileName = mediaType = rawBody = string.Empty;
            rawBytes.SetData(false, Array.Empty<byte>());
        }
    }
}