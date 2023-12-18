using System;
using SummerRest.Scripts.Attributes;
using SummerRest.Scripts.Models.Interfaces;
using UnityEngine;

namespace SummerRest.Scripts.Models
{
    [Serializable]
    internal class RequestParam
    {
        [field: SerializeField] public string Key { get; private set; }
        [field: SerializeReference, SerializedGenericField(typeof(bool), typeof(bool), typeof(string), typeof(float))] 
        public IRequestParamData Data { get; private set; }
    }
}