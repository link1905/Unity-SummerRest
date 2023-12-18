using System;
using UnityEngine;

namespace SummerRest.Scripts.Attributes
{
    public class SerializedGenericField : PropertyAttribute
    {
        public Type DefaultType { get; }
        public Type[] BaseTypes { get; }
        public SerializedGenericField(Type defaultType, params Type[] baseTypes)
        {
            DefaultType = defaultType;
            BaseTypes = baseTypes;
        }
    }
}