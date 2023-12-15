using System;
using MemoryPack;
using SummerRest.Scripts.Attributes;
using SummerRest.Scripts.DataStructures;
using SummerRest.Scripts.Models;
using UnityEngine;

namespace SummerRest.Scripts.Tests
{
    [Serializable]
    [MemoryPackable]
    public partial class RequestParamValue : IRequestParamValue
    {
        [field: SerializeField] public int A { get; private set; }
        [field: SerializeField] public int B { get; private set; }
    }
    public class TestBehaviour : MonoBehaviour
    {
        [SerializeField] private InterfaceContainer<RequestParamValue> testParam;
        //
        // [SerializeField, Inherits(typeof(IRequestParam), IncludeTypes =  new []{
        //     typeof(bool), typeof(string), typeof(float)
        // },
        // ShowNoneElement = false)] 
        // private TypeReference g = new(typeof(bool));
    }
}