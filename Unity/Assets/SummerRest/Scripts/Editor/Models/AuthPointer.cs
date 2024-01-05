using System;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class AuthPointer
    {
        [SerializeField] private string authKey;
        public static implicit operator string(AuthPointer p) => p.authKey;
    }
}