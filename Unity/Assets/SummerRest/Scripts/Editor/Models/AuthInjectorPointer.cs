using System;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class AuthInjectorPointer
    {
        [field: SerializeField] public AuthType Type { get; set; }
        [field: SerializeField] public string AuthId { get; set; }
    }
}