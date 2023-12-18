using System;
using UnityEngine;

namespace SummerRest.Scripts.Models
{
    [Serializable]
    internal class ApiVersion
    {
        [field: SerializeField] public string Origin { get; private set; }
    }
}