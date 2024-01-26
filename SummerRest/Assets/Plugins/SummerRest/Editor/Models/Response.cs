using System;
using System.Globalization;
using System.Net;
using SummerRest.Editor.DataStructures;
using UnityEngine;
using UnityEngine.Serialization;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// Storing values of a HTTP response (editor-only)
    /// </summary>
    [Serializable]
    public class Response
    {
        [SerializeField] private string lastCall;

        [SerializeField] private string error;
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

        [SerializeField] private KeyValue[] headers;
        public KeyValue[] Headers
        {
            get => headers;
            set => headers = value;
        }
        
        [SerializeField] private ResponseBody body;
        public ResponseBody Body => body;

        public void Clear()
        {
            lastCall = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            error = string.Empty;
            statusCode = HttpStatusCode.OK;
            headers = Array.Empty<KeyValue>();
            body.Clear();
        }
    }
}