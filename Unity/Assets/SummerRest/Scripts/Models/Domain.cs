using System;
using SummerRest.DataStructures;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    internal class Domain : EndPoint<Service>
    {
        [field: SerializeField] public EnableValue<ApiVersion> VersionOrigins { get; private set; }
        /// <summary>
        /// Set when change the active version in <see cref="VersionOrigins"/> 
        /// </summary>
        public ApiVersion ActiveVersion { get; private set; }
    }
}