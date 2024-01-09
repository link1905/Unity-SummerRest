using System;
using SummerRest.Editor.Models;
using SummerRest.Utilities.RequestComponents;
using UnityEngine;

namespace SummerRest.Samples.Behaviours
{
    public class TestMonoBehaviour : MonoBehaviour
    {
        [Serializable]
        private class CustomRequestBodyData : IRequestBodyData
        {
            [SerializeField] private string a;
            public string A => a;
            [SerializeField] private int b;
            public int B => b;
        }

        private void OnValidate()
        {
            requestBody.CacheValue(DataFormat.Xml);
        }

        [SerializeField] private AuthContainer authContainer;
        [SerializeField] private RequestBody requestBody;
    }
}