using System;
using System.Text;
using System.Xml.Serialization;
using SummerRest.Runtime.Attributes;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestComponents
{
    /// <summary>
    /// Http content type including 3 fields {charset, media type, boundary} based on https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Type <br/>
    /// This class also contains helpful components of a content type via <see cref="Headers"/> <see cref="Encodings"/> <see cref="MediaTypeNames"/>
    /// </summary>
    [Serializable]
    public struct ContentType
    {
        public static class Headers
        {
            public const string CharSet = "charset";
            public const string MediaType = "media-type";
            public const string Boundary = "boundary";
        }
        public static class Encodings
        {
            public const string Utf8 = "UTF-8";
            public const string Utf16 = "UTF-16";
            public const string UsAscii = "US-ASCII";
        }
        public static class MediaTypeNames
        {
            public static class Application
            {
                public const string Json = "application/json";
                public const string Octet = "application/octet-stream";
                public const string Soap = "application/soap+xml";
                public const string Xml = "application/xml";
                public const string WwwForm = "application/x-www-form-urlencoded";
            }

            public static class Multipart
            {
                public const string FormData = "multipart/form-data";
                public const string Mixed = "multipart/mixed";
            }

            public static class Text
            {
                public const string Html = "text/html";
                public const string Plain = "text/plain";
                public const string RichText = "text/richtext";
            }
        }

        public static class Commons
        {
            public static ContentType ApplicationJson => new(MediaTypeNames.Application.Json, Encodings.Utf8);
            public static ContentType ApplicationXml => new(MediaTypeNames.Application.Xml, Encodings.Utf8);
            public static ContentType TextPlain => new(MediaTypeNames.Text.Plain, Encodings.Utf8);
            public static ContentType Binary => new(MediaTypeNames.Application.Octet, Encodings.Utf8);
            public static string RandomBoundary { get; } = Encoding.UTF8.GetString(UnityWebRequest.GenerateBoundary());
            public static ContentType MultipartForm => new(MediaTypeNames.Multipart.FormData, Encodings.Utf8, RandomBoundary);
        }

        private static readonly NotImplementedException ImmutableError = new NotImplementedException("Content fields are immutable. Please create a new one using With method");

        [SerializeField, Defaults(Encodings.Utf8, Encodings.Utf16, Encodings.UsAscii)]
        private string charset;
        [XmlAttribute]
        public readonly string Charset { get => charset; set => throw ImmutableError; }

        [SerializeField, Defaults(
                    MediaTypeNames.Application.Json, MediaTypeNames.Application.WwwForm,
                    MediaTypeNames.Application.Soap, MediaTypeNames.Application.Xml, MediaTypeNames.Application.Octet,
                    MediaTypeNames.Text.Plain, MediaTypeNames.Text.RichText, MediaTypeNames.Text.Html,
                    MediaTypeNames.Multipart.FormData, MediaTypeNames.Multipart.Mixed
                )]
        private string mediaType;
        [XmlAttribute]
        public readonly string MediaType { get => mediaType; set => throw ImmutableError; }
        [SerializeField]
        private string boundary;
        [XmlAttribute]
        public readonly string Boundary { get => boundary;
            set => throw ImmutableError;
        }
        
        /// <summary>
        /// Content-type string formed from the 3 components
        /// </summary>
        public string FormedContentType { get; }
        public byte[] BoundaryBytes { get; }
        public static string FormedContentTypeTextFromComponents(string mediaType, string charset, string boundary)
        {
            var builder = new StringBuilder();
            builder.Append(mediaType);
            if (!string.IsNullOrEmpty(charset))
                builder.Append($"; {Headers.CharSet}=").Append(charset);
            if (!string.IsNullOrEmpty(boundary))
                builder.Append($"; {Headers.Boundary}=").Append(boundary);
            return builder.ToString();
        }

        public static byte[] FormBoundaryBytes(string boundary)
        {
            if (string.IsNullOrEmpty(boundary))
                return null;
            return Encoding.UTF8.GetBytes(boundary); 
        }

        public ContentType(string mediaType = null, string charset = null, string boundary = null)
        {
            this.mediaType = mediaType;
            this.charset = charset;
            this.boundary = boundary;
            //Create cached content-type text
            FormedContentType = FormedContentTypeTextFromComponents(mediaType, charset, boundary);
            BoundaryBytes = FormBoundaryBytes(boundary);
        }
        /// <summary>
        /// Create a new content type based on an existing one <br/>
        /// A null parameter means using the old value for the associated field
        /// </summary>
        /// <param name="newMediaType"></param>
        /// <param name="newCharset"></param>
        /// <param name="newBoundary"></param>
        /// <returns></returns>
        public readonly ContentType With(string newMediaType = null, string newCharset = null, string newBoundary = null)
        {
            newMediaType ??= MediaType;
            newCharset ??= Charset;
            newBoundary ??= Boundary;
            return new ContentType(newMediaType, newCharset, newBoundary);
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