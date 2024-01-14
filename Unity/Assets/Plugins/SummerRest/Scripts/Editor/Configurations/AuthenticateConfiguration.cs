using System.Linq;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Models;
using SummerRest.Editor.TypeReference;
using SummerRest.Runtime.Authenticate.TokenRepository;
using UnityEngine;

namespace SummerRest.Editor.Configurations
{
    //[CreateAssetMenu(menuName = "Summer/Rest/AuthenticateConfigurations", fileName = "AuthenticateConfigurations", order = 0)]
    public class AuthenticateConfiguration : ScriptableObject
    {
        [SerializeField] private AuthContainer[] auths;
        public AuthContainer[] AuthContainers => auths;
        [SerializeField, ClassTypeConstraint(typeof(IAuthDataRepository))] 
        private ClassTypeReference defaultTokenRepositoryType = new(typeof(PlayerPrefsAuthDataRepository));
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