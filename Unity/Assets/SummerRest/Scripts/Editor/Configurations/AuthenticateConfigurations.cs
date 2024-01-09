using System;
using System.Collections.Generic;
using System.Linq;
using SummerRest.Editor.Models;
using SummerRest.Runtime.Authenticate.TokenRepository;
using SummerRest.Utilities.DataStructures;
using TypeReferences;
using UnityEngine;
using UnityEngine.Serialization;

namespace SummerRest.Editor.Configurations
{
    [CreateAssetMenu(menuName = "Summer/Rest/AuthenticateConfigurations", fileName = "AuthenticateConfigurations", order = 0)]
    public class AuthenticateConfigurations : ScriptableObject
    {
        [SerializeField] private AuthContainer[] auths;
        public AuthContainer[] AuthContainers => auths;
        [FormerlySerializedAs("defaultRepositoryType")] [SerializeField, Inherits(typeof(IAuthDataRepository), ShowAllTypes = true, AllowInternal = true, ShortName = true)] 
        private TypeReference defaultTokenRepositoryType = new(typeof(PlayerPrefsAuthDataRepository));
        [SerializeField] private string[] previousKeys;

        private void OnValidate()
        {
            Cache();
        }

        private void Cache()
        {
            var authRepos =
                IGenericSingleton.GetSingleton<IAuthDataRepository, PlayerPrefsAuthDataRepository>(defaultTokenRepositoryType.Type);
            foreach (var key in previousKeys)
                authRepos.Delete(key);
            previousKeys = auths.Select(e => e.Cache(authRepos)).Where(e => !string.IsNullOrEmpty(e)).ToArray();
        }
    }
}