using System;
using System.Collections.Generic;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.Models;
using SummerRest.Runtime.Authenticate.TokenRepository;
using TypeReferences;
using UnityEngine;

namespace SummerRest.Editor.Configurations
{
    [CreateAssetMenu(menuName = "Summer/Rest/AuthenticateConfigurations", fileName = "AuthenticateConfigurations", order = 0)]
    public class AuthenticateConfigurations : ScriptableObject
    {
        [SerializeField] private AuthContainer[] auths;
        public AuthContainer[] AuthContainers => auths;
        [SerializeField] private AuthRepositoryContainer defaultRepository;
        
        [Serializable]
        public class AuthRepositoryContainer : InterfaceContainer<IAuthDataRepository>
        {
            [SerializeField, Inherits(typeof(IAuthDataRepository), ShowAllTypes = true, AllowInternal = true, ShortName = true)] 
            private TypeReference typeReference;
            public override Type Type => typeReference?.Type is null ? null : Type.GetType(typeReference.TypeNameAndAssembly);
        }
    }
}