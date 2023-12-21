using System;
using SummerRest.Attributes;
using SummerRest.Models.Interfaces;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    public class RequestParam
    {
        [field: SerializeField] public string Key { get; private set; }
        //[field: SerializeReference, SerializedGenericField(typeof(bool), typeof(bool), typeof(string), typeof(float))] 
        // public IRequestParamData Data { get; private set; }
    }
}