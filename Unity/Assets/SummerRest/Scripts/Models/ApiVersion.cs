using System;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    public class ApiVersion
    {
        [field: SerializeField] public string Origin { get; private set; }
    }
}