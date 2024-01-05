using System;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Scripts.Utilities.DataStructures;
using TypeReferences;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class AuthContainer : TextOrCustomData<Scripts.Utilities.RequestComponents.IAuthData, AuthContainer.BodyContainer>
    {
        [SerializeField] private string key;
        public string Key => key;
        [SerializeField, Inherits(typeof(IAuthAppender<>), ShowAllTypes = true, AllowInternal = true)] private TypeReference appenderType = new(typeof(BearerAuthAppender));
        [Serializable]
        public class BodyContainer : InterfaceContainer<Scripts.Utilities.RequestComponents.IAuthData>
        {
            [SerializeField, Inherits(typeof(Scripts.Utilities.RequestComponents.IAuthData), ShowAllTypes = true, AllowInternal = true)] 
            private TypeReference typeReference;
            public override Type Type => typeReference?.Type is null ? null : Type.GetType(typeReference.TypeNameAndAssembly);
        }
    }
}