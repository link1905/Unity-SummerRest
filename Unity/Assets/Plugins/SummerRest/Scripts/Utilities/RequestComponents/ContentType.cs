using System;
using System.Text;
using SummerRest.Utilities.Attributes;
using UnityEngine;

namespace SummerRest.Utilities.RequestComponents
{
    // [Serializable]
    // public struct ContentDisposition
    // {
    //     [field: SerializeField] public bool Inline { get; private set; }
    //     [field: SerializeField] public string Name { get; private set; }
    //     [field: SerializeField] public string FileName { get; private set; }
    // }
    
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
                public const string Pdf = "application/pdf";
                public const string Rtf = "application/rtf";
                public const string Soap = "application/soap+xml";
                public const string Xml = "application/xml";
                public const string Zip = "application/zip";
                public const string WwwForm = "application/x-www-form-urlencoded";
            }

            public static class Image
            {
                public const string Gif = "image/gif";
                public const string Jpeg = "image/jpeg";
                public const string Png = "image/png";
                public const string Tiff = "image/tiff";
            }
            
            public static class Audio
            {
                public const string Mpeg = "audio/mpeg";
                public const string Wav = "audio/wav";
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
                public const string Xml = "text/xml";
            }
        }
        
        [field: SerializeField, Defaults(Encodings.Utf8, Encodings.Utf16, Encodings.UsAscii)]
        public string Charset { get; private set; }

        [field: SerializeField, Defaults(
                    MediaTypeNames.Application.Json, MediaTypeNames.Application.WwwForm,
                    MediaTypeNames.Application.Soap, MediaTypeNames.Application.Xml, MediaTypeNames.Application.Octet,
                    MediaTypeNames.Text.Plain, MediaTypeNames.Text.RichText,
                    MediaTypeNames.Multipart.FormData, MediaTypeNames.Multipart.Mixed,
                    MediaTypeNames.Image.Jpeg, MediaTypeNames.Image.Png,
                    MediaTypeNames.Audio.Wav, MediaTypeNames.Audio.Mpeg
                )]
        public string MediaType { get; private set; }
        [field: SerializeField] public string Boundary { get; private set; }

        public string FormedContentType
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(MediaType);
                if (!string.IsNullOrEmpty(Charset))
                    builder.Append($"; {Headers.CharSet}=").Append(Charset);
                if (!string.IsNullOrEmpty(Boundary))
                    builder.Append($"; {Headers.Boundary}=").Append(Boundary);
                return builder.ToString();
            }
        }

        public ContentType(string mediaType = MediaTypeNames.Text.Plain, string charset = null, string boundary = null)
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