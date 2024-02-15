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
        public static implicit operator AuthContainer(AuthPointer p) => 
            SummerRestConfiguration.Instance.AuthContainers.FirstOrDefault(e => e.AuthKey == p.authKey);
        public static implicit operator AuthPointer(AuthContainer key) => key is not null ? new AuthPointer
        {
            authKey = key.AuthKey
        } : default;
 
        public bool ValidateToGenerate()
        {
            return (AuthContainer)this is not null;
        }
    }
}