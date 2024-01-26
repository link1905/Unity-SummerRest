using System;
using System.Linq;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.TypeReference;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// Let users request with their custom classes inheriting from <see cref="IRequestBodyData"/> 
    /// </summary>
    [Serializable]
    public class RequestBody : TextOrCustomData<RequestBodyType, IRequestBodyData, RequestBody.RequestBodyContainer>
    {
        [SerializeField] private string serializedData;
        [SerializeField] private FileReference file;
        [SerializeField] private MultipartFormRow[] form;

        public string SerializedData(DataFormat dataFormat, bool beauty)
        {
            switch (type)
            {
                case RequestBodyType.PlainText:
                    return text;
                case RequestBodyType.Data: 
                    return DefaultDataSerializer.StaticSerialize(body.Value, dataFormat, beauty);
                case RequestBodyType.MultipartForm:
                    return string.Empty;
            }
            return string.Empty;
        }
        public KeyValue[] SerializedForm => form.Select(e => e.SerializedPair).Where(e => e.HasValue).Select(e => e.Value).ToArray();

        [Serializable]
        public class RequestBodyContainer : InterfaceContainer<IRequestBodyData>
        {
            [SerializeField, ClassTypeConstraint(typeof(IRequestBodyData))] 
            private ClassTypeReference typeReference;
            public override Type Type => Type.GetType(typeReference.ClassRef);
        }

        public bool IsMultipart => type == RequestBodyType.MultipartForm;
        public ContentType? CacheValue(DataFormat dataFormat)
        {
            serializedData = SerializedData(dataFormat, true);
            switch (type)
            {
                case RequestBodyType.PlainText:
                    return ContentType.Commons.TextPlain;
                case RequestBodyType.Data:
                    switch (dataFormat)
                    {
                        case DataFormat.Json:
                            return ContentType.Commons.ApplicationJson;;
                        case DataFormat.PlainText:
                            return ContentType.Commons.TextPlain;
                        case DataFormat.Xml:
                            return ContentType.Commons.ApplicationXml;;
                    }
                    break;
                case RequestBodyType.MultipartForm:
                    return ContentType.Commons.MultipartForm;
            }
            return null;
        }
    }
}