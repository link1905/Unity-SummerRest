using System;
using UnityEngine;

namespace SummerRest.Runtime.RequestComponents
{
    [Serializable]
    public class BasicAuth : IAuthData
    {
        [field: SerializeField] public string Username { get; set; }
        [field: SerializeField] public string Password { get; set; }
    }
}