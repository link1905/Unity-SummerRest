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
    /// Points to an auth data (userId, token...) resolved by an <see cref="IAuthDataRepository"/> <br/> 
    /// In editor: used for generating <see cref="BaseAuthRequest{TRequest,TAuthAppender,TAuthData}"/>. It also contains the default value (string or <see cref="IAuthData"/>) for calling editor requests <seealso cref="AuthContainer.text"/>  <seealso cref="AuthContainer.body"/>
    /// </summary>
    [Serializable]
    public class AuthContainer : TextOrCustomData<AuthContainer.Type, IAuthData, AuthContainer.BodyContainer>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Key for resolving the auth value in runtime by using a <see cref="IAuthDataRepository"/> <seealso cref="IAuthAppender{TAuthAppender, TAuthData}"/> <seealso cref="IAuthAppender{TAuthAppender,TAuth}"/>
        /// </summary>
        [SerializeField] private string key;
        public string AuthKey => key;
        /// <summary>
        /// Type of <see cref="IAuthAppender{TAuthAppender, TAuthData}"/> for appending auth values into the request
        /// </summary>
        [SerializeField, ClassTypeConstraint(typeof(IAuthAppender<,>))] 
        private ClassTypeReference appenderType = new(typeof(BearerTokenAuthAppender));
        public string AppenderType => appenderType?.Type?.FullName;
        public string AuthDataType => AuthData?.FullName;
        [JsonIgnore] public System.Type AuthData
        {
            get
            {
                var selectedAppender = appenderType.Type;
                return selectedAppender?.GetInterface(typeof(IAuthAppender<,>).FullName).GenericTypeArguments[1];
            }
        }
        public enum Type
        {
            PlainText, Data
        }
        [JsonIgnore] public System.Type Appender => appenderType.Type;
        public object GetData()
        {
            if (type == Type.PlainText)
                return text;
            return body;
        }
        [Serializable]
        public class BodyContainer : InterfaceContainer<IAuthData>
        {
            public System.Type TypeBasedOnAppender { get; set; }
            public override System.Type Type => TypeBasedOnAppender;
        }
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            var selectedAppender = appenderType.Type;
            if (selectedAppender is null)
            {
                body.TypeBasedOnAppender = null;
                return;
            }
            // Use reflection to get the type of AuthData
            var typeOfAuthData = AuthData;
            // String => shift to text
            if (typeOfAuthData == typeof(string))
                type = Type.PlainText;
            else
            {
                type = Type.Data;
                body.TypeBasedOnAppender = typeOfAuthData;
            }
        }
    }
}