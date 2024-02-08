using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SummerRest.Runtime.RequestComponents
{
    [Serializable]
    public struct Account : IAuthData, IRequestBodyData
    {
        public string username;
        public string password;
    }
}