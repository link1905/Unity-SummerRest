using System;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    [Serializable]
    public struct Present<T>
    {
        [SerializeField] private bool hasValue;
        [SerializeField] private T value;
        public bool HasValue => hasValue;
        public T Value => value;
        public Present(bool hasValue, T value)
        {
            this.hasValue = hasValue;
            this.value = value;
        }

    }
}