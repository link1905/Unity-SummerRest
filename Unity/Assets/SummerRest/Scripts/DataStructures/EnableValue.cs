using System;
using UnityEngine;

namespace SummerRest.Scripts.DataStructures
{
    [Serializable]
    internal class EnableValue<T>
    {
        [field: SerializeField] public T Value { get; private set; }
        [field: SerializeField] public bool Enable { get; private set; }
    }
}