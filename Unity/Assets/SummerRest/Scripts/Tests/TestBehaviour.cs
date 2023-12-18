using System;
using MemoryPack;
using SummerRest.Scripts.DataStructures.Primitives;
using SummerRest.Scripts.Models.Interfaces;
using UnityEngine;

namespace SummerRest.Scripts.Tests
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
        [SerializeField] private float f;
    }
}