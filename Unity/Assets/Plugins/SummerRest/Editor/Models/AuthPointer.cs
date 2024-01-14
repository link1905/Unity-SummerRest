using System;
using System.Linq;
using Newtonsoft.Json;
using SummerRest.Editor.Configurations;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public struct AuthPointer
    {
        [SerializeField, JsonIgnore] private string authKey;
        public string AuthKey => authKey;
        public static implicit operator AuthContainer(AuthPointer p)
        {
            var authConfigure= SummerRestConfiguration.Instance.AuthenticateConfiguration;
            return authConfigure.AuthContainers.FirstOrDefault(e => e.AuthKey == p.authKey);
        }
        public static implicit operator AuthPointer(AuthContainer key) => key is not null ? new AuthPointer()
        {
            authKey = key.AuthKey
        } : default;
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {

        }
    }
}