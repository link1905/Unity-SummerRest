using System;
using SummerRest.Scripts.Utilities.DataStructures;
using SummerRest.Scripts.Utilities.Extensions;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine;

namespace SummerRest.Runtime.Parsers
{
    public class DefaultContentTypeParser : IContentTypeParser
    {
        public const string Json = "application/json";
        public const string Xml = "application/xml";
        public const string Plain = "text/plain";
        public const string Octet = "application/octet-stream";
        public const string WwwForm = "application/x-www-form-urlencoded";
        public const string CharSet = "charset";
        public const string MediaType = "media-type";
        public const string Boundary = "boundary";
        public const string UnityWebRequestContentTypeHeader = "Content-Type";
        public ContentType DefaultContentType { get; } = new(WwwForm);
        public string ContentTypeHeaderKey => UnityWebRequestContentTypeHeader;

        public DataFormat ParseDataFormatFromResponse(string contentTypeHeader)
        {
            var mediaType = ParseMediaTypeFromContentType(contentTypeHeader);
            switch (mediaType)
            {
                case Json:
                    return DataFormat.Json;
                case Xml:
                    return DataFormat.Xml;
                case Octet:
                    return DataFormat.Bson;
                case Plain:
                    return DataFormat.PlainText;
                default:
                    Debug.LogWarningFormat(
                        "Can not detect the content type of the response {0} -> automatically use {1}",
                        contentTypeHeader, DataFormat.PlainText);
                    return DataFormat.PlainText;
            }
        }

        public ContentType ParseContentTypeFromHeader(string contentTypeHeader)
        {
            if (string.IsNullOrEmpty(contentTypeHeader))
            {
                Debug.LogWarningFormat(
                    @"The header ""Content-Type"" is absent from the response -> automatically use {0}", Plain);
                return new ContentType();
            }

            string mediaType = null;
            string charSet = null;
            string boundary = null;
            var segment = new StringSegment(contentTypeHeader, ';');
            var charSetSpan = CharSet.AsSpan();
            var boundarySpan = Boundary.AsSpan();
            foreach (var entry in segment)
            {
                if (entry.Line.IsEmpty)
                    continue;
                if (entry.Line.SplitKeyValue(out var key, out var value))
                {
                    switch (key)
                    {
                        case var _ when key.SequenceEqual(charSetSpan):
                            charSet = value.ToString();
                            break;
                        case var _ when key.SequenceEqual(boundarySpan):
                            boundary = value.ToString();
                            break;
                    }
                }
                // Can not find "=" => media type
                else
                    mediaType = key.ToString();
            }

            if (string.IsNullOrEmpty(mediaType))
            {
                Debug.LogWarningFormat("Media type is absent from the content type {0} -> automatically use {1}",
                    contentTypeHeader, Plain);
                mediaType = Plain;
            }

            return new ContentType(mediaType, charSet, boundary);
        }

        private static string ParseMediaTypeFromContentType(
            string contentTypeHeader)
        {
            if (string.IsNullOrEmpty(contentTypeHeader))
            {
                Debug.LogWarningFormat(
                    @"The header ""Content-Type"" is absent from the response -> automatically use {0}", Plain);
                return Plain;
            }

            // Get part before ";"
            contentTypeHeader.AsSpan()
                .SplitKeyValue(out var mediaType, out _, ';');
            return mediaType.ToString();
        }
    }
}