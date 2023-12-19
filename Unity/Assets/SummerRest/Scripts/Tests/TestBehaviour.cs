using System;
using MemoryPack;
using SummerRest.Attributes;
using SummerRest.DataStructures;
using SummerRest.DataStructures.Primitives;
using SummerRest.Models;
using SummerRest.Models.Interfaces;
using UnityEngine;

namespace SummerRest.Tests
{
    [Serializable]
    [MemoryPackable]
    public partial class RequestParamData : IRequestParamData
    {
        public int A;
        public int B;
    }
    public partial class TestBehaviour : MonoBehaviour
    {
        [SerializeField] private OptionsArray<string> f;
        [SerializeField] private RestString restString;
        [SerializeField] private RestFloat restFloat;
        [SerializeField, Defaults("A", "B", "C")] private string defaultTest;
        private TestBehaviour Parent;
        [SerializeField, InheritOrCustom] private DataFormat dataFormat;
        [SerializedGenericField(typeof(RequestParamData)), SerializeReference]
        private IRequestParamData _value;
    }
}