using System;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.TypeReference;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class RequestBody : TextOrCustomData<IRequestBodyData, RequestBody.RequestBodyContainer>
    {
        [SerializeField] private string serializedData;
        public string SerializedData(DataFormat dataFormat, bool beauty) => 
            type == TextOrCustomDataType.PlainText ? text : body.Type is not null ? 
                DefaultDataSerializer.StaticSerialize(body.Value, dataFormat, beauty) : string.Empty;

        [Serializable]
        public class RequestBodyContainer : InterfaceContainer<IRequestBodyData>
        {
            [SerializeField, ClassTypeConstraint(typeof(IRequestBodyData))] 
            private ClassTypeReference typeReference;
            public override Type Type => typeReference?.Type is null ? null : Type.GetType(typeReference.ClassRef);
        }
        public void CacheValue(DataFormat dataFormat)
        {
            serializedData = SerializedData(dataFormat, true);
        }
    }
}