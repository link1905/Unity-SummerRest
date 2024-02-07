using System;
using System.Text;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Runtime.Authenticate.Appenders
{
    public class BasicAuthAppender : IAuthAppender<BasicAuthAppender, Account>
    {
        private const string AuthKeyword = "Basic";
        public void Append<TResponse>(Account authData, IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            if (string.IsNullOrEmpty(authData.username) || string.IsNullOrEmpty(authData.password))
            {
                Debug.LogWarningFormat("Basic auth information is null or empty");
                return;
            }
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{authData.username}:{authData.password}"));
            requestAdaptor.SetHeader("Authorization", $"{AuthKeyword} {credentials}");
        }
    }
}