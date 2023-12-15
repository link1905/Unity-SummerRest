using System;
using UnityEngine;

namespace SummerRest.Scripts.Attributes
{
    public class SerializedGenericField : PropertyAttribute
    {
        public Type[] BaseTypes { get; }
        public int DefaultIndex { get; }

        public SerializedGenericField(Type[] baseTypes, int defaultIndex)
        {
            BaseTypes = baseTypes;
            DefaultIndex = defaultIndex;
        }
    }
}