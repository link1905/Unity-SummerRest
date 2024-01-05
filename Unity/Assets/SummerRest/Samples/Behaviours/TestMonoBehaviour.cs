using System;
using SummerRest.Editor.Models;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine;

namespace SummerRest.Samples.Behaviours
{
    public class TestMonoBehaviour : MonoBehaviour
    {
        [Serializable]
        private class CustomRequestBodyData : IRequestBodyData
        {
            [SerializeField] private string a;
            [SerializeField] private int b;

            public CustomRequestBodyData(string a, int b)
            {
                this.a = a;
                this.b = b;
            }
        } 
        [SerializeField] private RequestBody testRequestBody;
        [SerializeReference] private IRequestBodyData CustomData = new CustomRequestBodyData("asda", 3);
    }
}