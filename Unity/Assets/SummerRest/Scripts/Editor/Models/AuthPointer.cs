using System;
using System.Linq;
using SummerRest.Editor.Configurations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public partial class AuthPointer
    {
        [SerializeField] private string authKey;

    }
#if UNITY_EDITOR
    public partial class AuthPointer : ISerializationCallbackReceiver
    {
        public static implicit operator AuthContainer(AuthPointer p)
        {
            var authConfigure= SummerRestConfigurations.Instance.AuthenticateConfigurations;
            return authConfigure.AuthContainers.FirstOrDefault(e => e.Key == p.authKey);
        }
        public static implicit operator AuthPointer(AuthContainer key) => new()
        {
            authKey = key.Key
        };
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {

        }
    }
#endif
}