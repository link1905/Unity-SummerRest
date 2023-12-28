using System;
using SummerRest.DataStructures.Containers;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    public partial class Domain : EndpointContainer
    {
        [SerializeField] private OptionsArray<string> versions;
        public string ActiveVersion => versions.Value;

        public override string TypeName => nameof(SummerRest.Models.Domain);
    }
#if UNITY_EDITOR
    public partial class Domain
    {
    }
#endif
}