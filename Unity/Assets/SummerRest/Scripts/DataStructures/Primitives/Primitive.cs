using System;
using SummerRest.Models.Interfaces;
using UnityEngine;

namespace SummerRest.DataStructures.Primitives
{
    [Serializable]
    public abstract class Primitive<T>
    {
        [SerializeField] private T value;
        public T Value
        {
            get => value;
            set => this.value = value;
        }
    }
}