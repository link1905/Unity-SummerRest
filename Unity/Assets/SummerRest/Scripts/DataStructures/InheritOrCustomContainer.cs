using System;
using UnityEngine;

namespace SummerRest.DataStructures
{
    [Serializable]
    public class InheritOrCustomContainer<T> where T : notnull
    {
        [SerializeField] private bool inherit = false;
        [field: SerializeField] private T value;
        public T Value => inherit ? default : value;
    }
}