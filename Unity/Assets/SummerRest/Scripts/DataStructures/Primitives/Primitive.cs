using System;
using SummerRest.Models.Interfaces;
using UnityEngine;

namespace SummerRest.DataStructures.Primitives
{
    [Serializable]
    public abstract class Primitive<T> : IRequestParamData
    {
        [field: SerializeField] public T Value { get; private set; }
    }
}