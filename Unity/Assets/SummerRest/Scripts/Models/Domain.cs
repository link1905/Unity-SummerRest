using System;
using SummerRest.DataStructures;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    public class Domain : EndPointContainer
    {
        [SerializeField] private OptionsArray<string> versions;
        public string ActiveVersion => versions.Value;
    }
}