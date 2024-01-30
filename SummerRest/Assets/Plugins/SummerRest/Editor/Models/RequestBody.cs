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
        /// Only get texts
        /// </summary>
        public KeyValue[] TextSections => form.Where(e => e.Type == MultipartFormRowType.PlainText)
            .Select(e => e.Pair).ToArray();
        public IEnumerable<IMultipartFormSection> FormSections 
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

        public ContentType? CacheValue()
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
                    return ContentType.Commons.MultipartForm;
            }
            return null;
        }
    }
}