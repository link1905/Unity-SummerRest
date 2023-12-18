using System;
using UnityEngine;

namespace SummerRest.Scripts.Models
{
    [Serializable]
    internal class RequestHeader
    {
        [field: SerializeField] public string Key { get; private set; }
        [field: SerializeField] public string Value { get; private set; }
    }
}