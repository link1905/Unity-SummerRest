using System;
using System.Linq;
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
        public string AuthKey => authKey;
        public static implicit operator AuthContainer(AuthPointer p)
        {
            var authConfigure= SummerRestConfiguration.Instance.AuthContainers;
            return authConfigure.FirstOrDefault(e => e.AuthKey == p.authKey);
        }
        public static implicit operator AuthPointer(AuthContainer key) => key is not null ? new AuthPointer
        {
            authKey = key.AuthKey
        } : default;
    }
}