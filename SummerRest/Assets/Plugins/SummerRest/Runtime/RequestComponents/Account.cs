using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SummerRest.Runtime.RequestComponents
{
    [Serializable]
    public struct Account : IAuthData
    {
        public string username;
        public string password;
    }
}