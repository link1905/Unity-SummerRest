using System;
using System.Xml.Serialization;
using SummerRest.Editor.DataStructures;
using SummerRest.Editor.TypeReference;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    /// <summary>
    /// Points to an auth data (userId, token...) resolved by an <see cref="Runtime.Authenticate.Repositories.ISecretRepository"/> <br/> 
    /// It contains the default value (string or <see cref="IAuthData"/>) for calling editor requests <seealso cref="AuthContainer.text"/>  <seealso cref="AuthContainer.body"/>
    /// </summary>
    [Serializable]
    public class AuthContainer : TextOrCustomData<AuthContainer.AuthType, IAuthData, AuthContainer.BodyContainer>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Key for resolving the auth value in runtime by using a <see cref="Runtime.Authenticate.Repositories.ISecretRepository"/> <seealso cref="IAuthAppender{TAuthAppender, TAuthData}"/> <seealso cref="IAuthAppender{TAuthAppender,TAuth}"/>
        /// </summary>
        [SerializeField] private string key;
        [XmlAttribute]
        public string AuthKey
        {
            get => key;
            set => key = value;
        }

        /// <summary>
        /// Type of <see cref="IAuthAppender{TAuthAppender, TAuthData}"/> for appending auth values into the request
        /// </summary>
        [SerializeField, ClassTypeConstraint(typeof(IAuthAppender<,>))] 
        private ClassTypeReference appenderType = new(typeof(BearerTokenAuthAppender));
        [XmlAttribute]
        public string AppenderType
        {
            get => appenderType?.Type?.FullName;
            set => throw new NotImplementedException();
        }

        [XmlAttribute]
        public string AuthDataType
        {
            get => AuthData?.FullName;
            set => throw new NotImplementedException();
        }
        private Type AuthData
        {
            get
            {
                var selectedAppender = appenderType.Type;
                return selectedAppender?.GetInterface(typeof(IAuthAppender<,>).FullName).GenericTypeArguments[1];
            }
        }
        public enum AuthType
        {
            PlainText, Data
        }
        [XmlAttribute] public System.Type Appender => appenderType.Type;
        public object GetData()
        {
            if (type == AuthType.PlainText)
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
                type = AuthType.PlainText;
            else
            {
                type = AuthType.Data;
                body.TypeBasedOnAppender = typeOfAuthData;
            }
        }
    }
}