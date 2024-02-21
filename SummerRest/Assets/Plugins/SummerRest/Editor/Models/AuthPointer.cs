using System;
using System.Linq;
using System.Security.Policy;
using SummerRest.Editor.Configurations;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// Point to a <see cref="AuthContainer"/> inside <see cref="SummerRestConfiguration"/> <br/>
    /// </summary>
    [Serializable]
    public struct AuthPointer
    {
        [SerializeField] private string authKey;
        public readonly string AuthKey => authKey;
        public AuthContainer Cache()
        {
            var refKey = authKey;
            var container = SummerRestConfiguration.Instance.AuthContainers.FirstOrDefault(e => e.AuthKey == refKey);
            return container;
        }

        public static implicit operator AuthPointer(AuthContainer key) => key is not null ? new AuthPointer
        {
            authKey = key.AuthKey
        } : default;
    }
}