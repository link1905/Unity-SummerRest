using System;
using SummerRest.Editor.DataStructures;
using SummerRest.Runtime.Parsers;
using SummerRest.Utilities.RequestComponents;
using TypeReferences;
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
            [SerializeField, Inherits(typeof(IRequestBodyData), ShowAllTypes = true, AllowInternal = true, ShortName = true)] private TypeReference typeReference;
            public override Type Type => typeReference?.Type is null ? null : Type.GetType(typeReference.TypeNameAndAssembly);
        }
        public void CacheValue(DataFormat dataFormat)
        {
            serializedData = SerializedData(dataFormat, true);
        }
    }
}