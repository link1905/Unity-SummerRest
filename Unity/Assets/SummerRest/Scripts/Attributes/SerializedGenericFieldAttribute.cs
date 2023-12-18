using System;
using UnityEngine;

namespace SummerRest.Attributes
{
    public class SerializedGenericFieldAttribute : PropertyAttribute
    {
        public Type DefaultType { get; }
        public Type[] BaseTypes { get; }
        public SerializedGenericFieldAttribute(Type defaultType, params Type[] baseTypes)
        {
            DefaultType = defaultType;
            BaseTypes = baseTypes;
        }
    }
}