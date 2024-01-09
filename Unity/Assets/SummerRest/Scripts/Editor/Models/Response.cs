using System;
using System.Globalization;
using System.Net;
using SummerRest.Editor.DataStructures;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class Response
    {
        [SerializeField] private string dateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        [SerializeField] private HttpStatusCode statusCode = HttpStatusCode.OK;
        public HttpStatusCode StatusCode => statusCode;
        [SerializeField] private KeyValue[] headers;
        public KeyValue[] Header
        {
            get => headers;
            set => headers = value;
        }
        [SerializeField] private string body;
        public string Body => body;
    }
}