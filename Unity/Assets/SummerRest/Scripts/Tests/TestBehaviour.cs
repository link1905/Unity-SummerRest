using System;
using MemoryPack;
using SummerRest.DataStructures.Primitives;
using SummerRest.Models.Interfaces;
using UnityEngine;

namespace SummerRest.Tests
{
    [Serializable]
    [MemoryPackable]
    public partial class RequestParamData : IRequestParamData
    {
        [field: SerializeField] public int A { get; private set; }
        [field: SerializeField] public int B { get; private set; }
    }
    public class TestBehaviour : MonoBehaviour
    {
        [SerializeField] private RestString restString;
        [SerializeField] private RestFloat c;
        [SerializeField] private float f;
    }
}