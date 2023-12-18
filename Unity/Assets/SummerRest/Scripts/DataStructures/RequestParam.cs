using System;
using SummerRest.Scripts.Attributes;
using SummerRest.Scripts.DataStructures;
using TypeReferences;
using UnityEngine;
namespace RestSourceGenerator.Tests.Samples
{
    public partial class RequestParam
    {
        [SerializeField] private ValueContainer valueContainer;
        public IRequestParamValue Value => valueContainer.Value;
        public Type ValueType => valueContainer.Type;
        [Serializable]
        public class ValueContainer : InterfaceContainer<RestSourceGenerator.Tests.Samples.IRequestParamValue>
        {
            [SerializeField, Inherits(typeof(bool), typeof(string), typeof(float))]
            private TypeReference typeRef = new(typeof(bool));
        }
    }
}