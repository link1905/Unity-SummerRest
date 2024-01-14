using System;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    [Serializable]
    public class OptionsArray<T> 
    {
        [SerializeField, HideInInspector] private T[] values;
        [SerializeField, HideInInspector] private int selectedIndex;
        public T[] Values
        {
            get => values;
            set => values = value;
        }

        public T Value => selectedIndex >= 0 && selectedIndex < values.Length ? values[selectedIndex] : default;
    }
}