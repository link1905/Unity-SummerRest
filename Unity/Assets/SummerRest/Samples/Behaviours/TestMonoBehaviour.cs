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
            [SerializeField] private int b;
        }
    }
}