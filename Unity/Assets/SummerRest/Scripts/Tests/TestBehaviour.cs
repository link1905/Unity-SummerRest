using System;
using System.Collections.Generic;
using UnityEngine;

namespace SummerRest.Tests
{
    [Serializable]
    public class A
    {
        public C C;
    }
    [Serializable]
    public class C
    {
        public string D;
    }
    public class TestBehaviour : MonoBehaviour
    {
        [SerializeField] public List<A> request;
    }
}