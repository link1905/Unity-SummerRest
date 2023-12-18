using System;
using UnityEngine;

namespace SummerRest.Scripts.Models
{
    [Serializable]
    public class ContentType
    {
        [field: SerializeField] public string CharSet { get; private set; }
        [field: SerializeField] public string MediaType { get; private set; }
        [field: SerializeField] public string Boundary { get; private set; }
    }
}