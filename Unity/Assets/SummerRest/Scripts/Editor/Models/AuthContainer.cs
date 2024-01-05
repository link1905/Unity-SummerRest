using System;
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
        public class BodyContainer : InterfaceContainer<Scripts.Utilities.RequestComponents.IAuthData>
        {
            [SerializeField, Inherits(typeof(Scripts.Utilities.RequestComponents.IAuthData))] private TypeReference typeReference;
            public override Type Type => typeReference?.Type is null ? null : Type.GetType(typeReference.TypeNameAndAssembly);
        }
    }
}