using System;
using SummerRest.DataStructures.Containers;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    public class Domain : EndpointContainer
    {
        [SerializeField] private OptionsArray<string> versions;
        public string ActiveVersion => versions.Value;
        public override string TypeName => nameof(Domain);
    }
}