using System;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class BytesBody
    {
        [SerializeField] private bool isImage;
        [SerializeField] private byte[] data;

        public void SetData(bool inIsImage, byte[] inData)
        {
            isImage = inIsImage;
            data = inData;
        }
    }
}