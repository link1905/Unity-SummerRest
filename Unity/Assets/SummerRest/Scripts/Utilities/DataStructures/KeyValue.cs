using System;
using UnityEngine;

namespace SummerRest.Scripts.Utilities.DataStructures
{
    [Serializable]
    public class KeyValue
    {
        [SerializeField] private string key;
        public string Key => key;
        [SerializeField] private string value;
        public string Value => value;
        public void Deconstruct(out string outKey, out string outValue)
        {
            outKey = Key;
            outValue = Value;
        }
    }
}