using System;
using System.Text;
using SummerRest.Scripts.Utilities.Attributes;
using UnityEngine;

namespace SummerRest.Scripts.Utilities.RequestComponents
{
    [Serializable]
    public class ContentType
    {
        [field: SerializeField, Defaults("UTF-8", "UTF-16", "US-ASCII")]
        public string Charset { get; private set; }
        [field: SerializeField, Defaults(
                    "application/json", "application/xml", "application/octet-stream",
                    "multipart/form-data", "multipart/mixed",
                    "text/plain", "text/xml",
                    "image/jpeg", "image/png",
                    "audio/mpeg", "audio/wav")]
        public string MediaType { get; private set; }
        [field: SerializeField] public string Boundary { get; private set; }

        public string FormedContentType
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(MediaType);
                if (!string.IsNullOrEmpty(Charset))
                    builder.Append("; charset=").Append(Charset);
                if (!string.IsNullOrEmpty(Boundary))
                    builder.Append("; boundary=").Append(Boundary);
                return builder.ToString();
            }
        }
        public ContentType(string mediaType = "text/plain", string charset = null, string boundary = null)
        {
            Charset = charset;
            MediaType = mediaType;
            Boundary = boundary;
        }
        public ContentType With(string mediaType = null, string charset = null, string boundary = null)
        {
            mediaType ??= MediaType;
            charset ??= Charset;
            boundary ??= Boundary;
            return new ContentType(mediaType, charset, boundary);
        }
        public bool Equals(ContentType other)
        {
            return FormedContentType == other.FormedContentType;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Charset, MediaType, Boundary);
        }
    }
}