using System;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    /// <summary>
    /// C# 9.0 Generic structs are not fully compatible with returning nullable methods and <see cref="Nullable{T}"/> is not serializable<br/>
    /// Wrapped in this class working as <see cref="Nullable{T}"/> for both class and struct<br/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public struct Present<T>
    {
        [SerializeField] private bool hasValue;
        [SerializeField] private T value;
        public bool HasValue
        {
            get => hasValue;
            set => hasValue = value;
        }

        public T Value
        {
            get => value;
            set => this.value = value;
        }

        public Present(bool hasValue, T value)
        {
            this.hasValue = hasValue;
            this.value = value;
        }
        public Present(T value)
        {
            this.hasValue = true;
            this.value = value;
        }
        public static Present<T> Absent => new();
    }
}