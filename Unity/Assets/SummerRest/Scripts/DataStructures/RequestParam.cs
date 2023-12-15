using System;
using SummerRest.Scripts.Attributes;
using TypeReferences;
using UnityEngine;

namespace SummerRest.Scripts.DataStructures
{
    [Serializable]
    internal partial class RequestParam
    {
        [SerializedGenericField(typeof(bool), typeof(bool), typeof(string), typeof(float))]
        private IRequestParamValue _value;
    }
    
    
    internal partial class RequestParam
    {
        //[SerializedGenericField(typeof(bool), typeof(bool), typeof(string), typeof(float))] private IRequestParamValue _value;
        //Generated
        [SerializeField] private ValueContainer valueContainer;
        public IRequestParamValue Value => valueContainer.Value;
        public Type ValueType => valueContainer.Type;
        [Serializable]
        public class ValueContainer : InterfaceContainer<SummerRest.Scripts.Models.IRequestParamValue>
        {
            [SerializeField, Inherits(typeof(bool), IncludeTypes = new []{typeof(float), typeof(string)})] 
            private TypeReference typeRef = new(typeof(bool));
        }
        //
    }
}