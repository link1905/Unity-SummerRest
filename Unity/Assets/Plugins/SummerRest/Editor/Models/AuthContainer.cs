using System;
using Newtonsoft.Json;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.TypeReference;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Authenticate.TokenRepositories;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// Points to an auth data (userId, token...) resolved by an <see cref="IAuthDataRepository{TSelf}"/> <br/> 
    /// In editor: used for generating <see cref="BaseAuthRequest{TRequest,TAuthAppender}"/>. It also contains the default value (string or <see cref="IAuthData"/>) for calling editor requests <seealso cref="AuthContainer.text"/>  <seealso cref="AuthContainer.body"/>
    /// </summary>
    [Serializable]
    public class AuthContainer : TextOrCustomData<IAuthData, AuthContainer.BodyContainer>
    {
        /// <summary>
        /// Key for resolving the auth value in runtime by using a <see cref="IAuthDataRepository{TSelf}"/> <seealso cref="IAuthAppender{TAuthAppender}"/> <seealso cref="IAuthAppender{TAuthAppender}"/>
        /// </summary>
        [SerializeField] private string key;
        public string AuthKey => key;
        /// <summary>
        /// Type of <see cref="IAuthAppender{TAuthAppender}"/> for appending auth values into the request
        /// </summary>
        [SerializeField, ClassTypeConstraint(typeof(IAuthAppender))] 
        private ClassTypeReference appenderType = new(typeof(BearerTokenAuthAppender));
        public string AppenderType => appenderType?.Type?.FullName;
        [JsonIgnore] public Type Appender => appenderType.Type;
        public T GetData<T>()
        {
            if (type == TextOrCustomDataType.PlainText)
                return (T)(object)text;
            return (T)(object)body;
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