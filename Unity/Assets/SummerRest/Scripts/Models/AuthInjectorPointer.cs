using System;
using UnityEngine;

namespace SummerRest.Scripts.Models
{
    [Serializable]
    internal class AuthInjectorPointer
    {
        [field: SerializeField] public AuthType Type { get; set; }
        [field: SerializeField] public string AuthId { get; set; }
    }
}