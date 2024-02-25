using System;
using System.Collections.Generic;
using System.Linq;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.TypeReference;
using SummerRest.Editor.Utilities;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// Let users request with their custom classes inheriting from <see cref="IRequestBodyData"/> 
    /// </summary>
    [Serializable]
    public class RequestBody : TextOrCustomData<RequestBodyType, IRequestBodyData, RequestBody.RequestBodyContainer>
    {
        [SerializeField] private string serializedData;
        [SerializeField] private bool isGenerated = true;
        public bool Generated => isGenerated;
        [SerializeField] private DataFormat textFormat;
        [SerializeField] private MultipartFormRow[] form;

        public string SerializedData(bool beauty)
        {
            switch (type)
            {
                case RequestBodyType.Text:
                    return text;
                case RequestBodyType.Data: 
                    return IDataSerializer.Current.Serialize(body.Value, textFormat, beauty);
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// Completely get text fields <br/>
        /// File fields are represented by null value  
        /// </summary>
        public KeyValue[] RestrictedSections(bool allowText) => form
            .Select(e => e.Pair(allowText)).ToArray();
        public IEnumerable<IMultipartFormSection> AllSections 
            => form.Select(e => e.FormSection).Where(e => e is not null);
        [Serializable]
        public class RequestBodyContainer : InterfaceContainer<IRequestBodyData>
        {
            [SerializeField, ClassTypeConstraint(typeof(IRequestBodyData))] 
            private ClassTypeReference typeReference;
            public override Type Type => Type.GetType(typeReference.ClassRef);
        }

        public bool IsMultipart => type == RequestBodyType.MultipartForm;
        public DataFormat DataFormat => textFormat;

        private ContentType ContentTypeByTextFormat
        {
            get
            {
                switch (textFormat)
                {
                    case DataFormat.Json:
                        return ContentType.Commons.ApplicationJson;
                    case DataFormat.PlainText:
                        return ContentType.Commons.TextPlain;
                    case DataFormat.Xml:
                        return ContentType.Commons.ApplicationXml;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public ContentType? CacheValue(ContentType current)
        {
            serializedData = SerializedData(true);
            switch (type)
            {
                case RequestBodyType.Text:
                    if (string.IsNullOrEmpty(text))
                        return null;
                    textFormat = text.DetectFormat();
                    return ContentTypeByTextFormat;
                case RequestBodyType.Data:
                    return ContentTypeByTextFormat;
                case RequestBodyType.MultipartForm:
                    var result = ContentType.Commons.MultipartForm;
                    // Take current boundary if it is reusable
                    if (!string.IsNullOrEmpty(current.Boundary))
                        result = result.With(newBoundary: current.Boundary);
                    return result;
            }
            return null;
        }
    }
}