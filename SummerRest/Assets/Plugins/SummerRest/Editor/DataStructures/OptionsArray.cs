using System;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    /// <summary>
    /// Wrapping an array with an only selected value 
    /// </summary>
    [Serializable]
    public class OptionsArray<T> 
    {
        [SerializeField, HideInInspector] private T[] values;
        /// <summary>
        /// Points to the selected value
        /// </summary>
        [SerializeField, HideInInspector] private int selectedIndex;
        public T[] Values
        {
            get => values;
            set => values = value;
        }

        public T Value => selectedIndex >= 0 && selectedIndex < values.Length ? values[selectedIndex] : default;
    }
}