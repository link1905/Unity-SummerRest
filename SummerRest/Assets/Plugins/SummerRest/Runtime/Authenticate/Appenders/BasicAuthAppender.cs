using System;
using System.Text;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Runtime.Authenticate.Appenders
{
    public class BasicAuthAppender : IAuthAppender<BasicAuthAppender, BasicAuth>
    {
        private const string AuthKeyword = "Basic";
        public void Append<TResponse>(BasicAuth authData, IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            if (authData is null || string.IsNullOrEmpty(authData.Username) || string.IsNullOrEmpty(authData.Password))
            {
                Debug.LogWarningFormat("Basic auth information is null or empty");
                return;
            }
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{authData.Username}:{authData.Password}"));
            requestAdaptor.SetHeader("Authorization", $"{AuthKeyword} {credentials}");
        }
    }
}