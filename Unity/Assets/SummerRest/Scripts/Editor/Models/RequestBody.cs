using System;
using SummerRest.Runtime.Parsers;
using SummerRest.Scripts.Utilities.DataStructures;
using SummerRest.Scripts.Utilities.RequestComponents;
using TypeReferences;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class RequestBody : TextOrCustomData<IRequestBodyData, RequestBody.RequestBodyContainer>
    {
        public string SerializedData => type == TextOrCustomDataType.PlainText ? text : DefaultDataSerializer.StaticSerialize(body.Value, DataFormat.Json);

        [Serializable]
        public class RequestBodyContainer : InterfaceContainer<IRequestBodyData>
        {
            [SerializeField, Inherits(typeof(IRequestBodyData), ShowAllTypes = true, AllowInternal = true, ShortName = true)] private TypeReference typeReference;
            public override Type Type => typeReference?.Type is null ? null : Type.GetType(typeReference.TypeNameAndAssembly);
        }
    }
}