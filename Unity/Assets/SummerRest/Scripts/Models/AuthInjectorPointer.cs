using System;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    public class AuthInjectorPointer
    {
        [field: SerializeField] public AuthType Type { get; set; }
        [field: SerializeField] public string AuthId { get; set; }
    }
}