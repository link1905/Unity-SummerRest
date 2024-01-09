using System;
using SummerRest.Editor.DataStructures;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Authenticate.TokenRepository;
using SummerRest.Utilities.RequestComponents;
using TypeReferences;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class AuthContainer : TextOrCustomData<IAuthData, AuthContainer.BodyContainer>
    {
        [SerializeField] private string key;
        public string AuthKey => key;
        [SerializeField, Inherits(typeof(IAuthAppender), ShowAllTypes = true, AllowInternal = true, ShowNoneElement = false, ShortName = true)] 
        private TypeReference appenderType = new(typeof(BearerTokenAuthAppender));
        public string AppenderType => appenderType?.Type?.FullName;
        public string Cache(IAuthDataRepository authDataRepository)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            if (type == TextOrCustomDataType.PlainText)
                authDataRepository.Save(key, text);
            else
                authDataRepository.Save(key, body);
            return key;
        }
        [Serializable]
        public class BodyContainer : InterfaceContainer<IAuthData>
        {
            [SerializeField, Inherits(typeof(IAuthData), ShowAllTypes = true, AllowInternal = true, ShortName = true)] 
            private TypeReference typeReference;
            public override Type Type => typeReference?.Type is null ? null : Type.GetType(typeReference.TypeNameAndAssembly);
        }
    }
}