using System;
using SummerRest.Scripts.Models.Interfaces;
using UnityEngine;

namespace SummerRest.Scripts.DataStructures.Primitives
{
    [Serializable]
    public abstract class Primitive<T> : IRequestParamData
    {
        [field: SerializeField] public T Value { get; private set; }
    }
    [Serializable]
    public class RestFloat : Primitive<float>
    { }
    [Serializable]
    public class RestInt : Primitive<float>
    { }
    [Serializable]
    public class RestString : Primitive<string>
    { }
    [Serializable]
    public class RestBool : Primitive<bool>
    { }
}