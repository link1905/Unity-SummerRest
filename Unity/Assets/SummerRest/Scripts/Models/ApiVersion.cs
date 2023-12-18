using System;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    internal class ApiVersion
    {
        [field: SerializeField] public string Origin { get; private set; }
    }
}