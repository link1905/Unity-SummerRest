using System;
using System.Globalization;
using System.Net;
using SummerRest.Editor.Attributes;
using SummerRest.Editor.DataStructures;
using UnityEngine;
using UnityEngine.Serialization;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// Storing values of a HTTP response (editor-only)
    /// </summary>
    public class Response : ScriptableObject
    {
        [SerializeField, ReadonlyText] private string lastCall;

        [SerializeField, ReadonlyText] private string error;
        public string Error
        {
            get => error;
            set => error = value;
        }

        [SerializeField] private HttpStatusCode statusCode = HttpStatusCode.OK;
        public HttpStatusCode StatusCode
        {
            get => statusCode;
            set => statusCode = value;
        }

        [SerializeField] private KeyValue[] responseHeaders;
        public KeyValue[] Headers
        {
            get => responseHeaders;
            set => responseHeaders = value;
        }
        
        [SerializeField] private ResponseBody body;
        public ResponseBody Body => body;

        public void Clear()
        {
            lastCall = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            error = string.Empty;
            statusCode = HttpStatusCode.OK;
            responseHeaders = Array.Empty<KeyValue>();
            body.Clear();
        }
    }
}