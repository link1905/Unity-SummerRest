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
        [field: SerializeField] public string Value { get; private set; }
    }
}