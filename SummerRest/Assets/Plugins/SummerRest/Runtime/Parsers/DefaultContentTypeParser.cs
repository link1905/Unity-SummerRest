using System;
using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.Extensions;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;
using ContentType = SummerRest.Runtime.RequestComponents.ContentType;

namespace SummerRest.Runtime.Parsers
{
    public class DefaultContentTypeParser : IContentTypeParser
    {
        public const string UnityWebRequestContentTypeHeader = "Content-Type";
        public ContentType DefaultContentType { get; } = new(ContentType.MediaTypeNames.Application.WwwForm);
        public string ContentTypeHeaderKey => UnityWebRequestContentTypeHeader;

        public DataFormat ParseDataFormatFromResponse(string contentTypeHeader)
        {
            var mediaType = ParseMediaTypeFromContentType(contentTypeHeader);
            switch (mediaType)
            {
                case ContentType.MediaTypeNames.Application.Json:
                    return DataFormat.Json;
                case ContentType.MediaTypeNames.Application.Xml or ContentType.MediaTypeNames.Application.Soap:
                    return DataFormat.Xml;
                case ContentType.MediaTypeNames.Text.Plain or ContentType.MediaTypeNames.Text.RichText:
                    return DataFormat.PlainText;
                default:
                    Debug.LogWarningFormat(
                        "Can not detect the content type of the response {0} -> automatically use {1}",
                        contentTypeHeader, DataFormat.PlainText);
                    return DataFormat.PlainText;
            }
        }

        public static string ExtractFileName(string contentDisposition)
        {
            if (!string.IsNullOrEmpty(contentDisposition))
            {
                int startIndex = contentDisposition.IndexOf("filename=", StringComparison.InvariantCultureIgnoreCase);
                if (startIndex != -1)
                {
                    startIndex += 9; // "filename=".Length
                    int endIndex = contentDisposition.IndexOf(";", startIndex, StringComparison.InvariantCultureIgnoreCase);
                    if (endIndex == -1)
                    {
                        endIndex = contentDisposition.Length;
                    }
                    string fileName = contentDisposition.Substring(startIndex, endIndex - startIndex);
                    return fileName.Trim('\"');
                }
            }

            // If no filename found, return a default name or handle it accordingly
            return "UnknownFileName";
        }
        
        public ContentType ParseContentTypeFromHeader(string contentTypeHeader)
        {
            if (string.IsNullOrEmpty(contentTypeHeader))
            {
                Debug.LogWarningFormat(
                    @"The header ""Content-Type"" is absent from the response -> automatically use {0}", ContentType.MediaTypeNames.Text.Plain);
                return new ContentType();
            }

            string mediaType = null;
            string charSet = null;
            string boundary = null;
            var segment = new StringSegment(contentTypeHeader, ';');
            var charSetSpan = ContentType.Headers.CharSet.AsSpan();
            var boundarySpan = ContentType.Headers.Boundary.AsSpan();
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
                    contentTypeHeader, ContentType.MediaTypeNames.Text.Plain);
                mediaType = ContentType.MediaTypeNames.Text.Plain;
            }

            return new ContentType(mediaType, charSet, boundary);
        }

        private static string ParseMediaTypeFromContentType(
            string contentTypeHeader)
        {
            if (string.IsNullOrEmpty(contentTypeHeader))
            {
                Debug.LogWarningFormat(
                    @"The header ""Content-Type"" is absent from the response -> automatically use {0}", ContentType.MediaTypeNames.Text.Plain);
                return ContentType.MediaTypeNames.Text.Plain;
            }

            // Get part before ";"
            contentTypeHeader.AsSpan()
                .SplitKeyValue(out var mediaType, out _, ';');
            return mediaType.ToString();
        }
    }
}