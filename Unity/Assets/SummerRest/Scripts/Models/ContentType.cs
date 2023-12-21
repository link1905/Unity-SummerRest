using System;
using System.Runtime.InteropServices;
using SummerRest.Attributes;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    public class ContentType
    {
        [field: SerializeField, Defaults("UTF-8", "UTF-16", "US-ASCII")] 
        public string CharSet { get; private set; }
        [field: SerializeField, Defaults(
                    "application/json", "application/xml", "application/octet-stream", 
                    "multipart/form-data", "multipart/mixed", 
                    "text/plain", "text/xml", 
                    "image/jpeg", "image/png", 
                    "audio/mpeg", "audio/wav")] 
        public string MediaType { get; private set; }
        [field: SerializeField] public string Boundary { get; private set; }

        // public static readonly string[] DefaultCharsets = {
        //     "UTF-8", "UTF-16", "US-ASCII",
        // };
    }
}