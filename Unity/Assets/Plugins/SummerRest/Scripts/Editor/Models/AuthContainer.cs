using System;
using Newtonsoft.Json;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.TypeReference;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Authenticate.TokenRepository;
using SummerRest.Utilities.RequestComponents;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    [Serializable]
    public class AuthContainer : TextOrCustomData<IAuthData, AuthContainer.BodyContainer>
    {
        [SerializeField] private string key;
        public string AuthKey => key;
        [SerializeField, ClassTypeConstraint(typeof(IAuthAppender))] 
        private ClassTypeReference appenderType = new(typeof(BearerTokenAuthAppender));
        public string AppenderType => appenderType?.Type?.FullName;
        [JsonIgnore] public Type Appender => appenderType.Type; 
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
            [SerializeField, ClassTypeConstraint(typeof(IAuthData))] 
            private ClassTypeReference typeReference;
            public override Type Type => typeReference?.Type is null ? null : Type.GetType(typeReference.ClassRef);
        }
    }
}