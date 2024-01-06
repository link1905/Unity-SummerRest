using System;
using UnityEngine;

namespace SummerRest.Scripts.Utilities.DataStructures
{
    [Serializable]
    public struct KeyValue
    {
        [SerializeField] private string key;
        public string Key => key;
        [SerializeField] private string value;
        public string Value => value;
        public KeyValue(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
        public KeyValue Clone()
        {
            return new KeyValue(key, value);
        }
        public void Deconstruct(out string outKey, out string outValue)
        {
            outKey = Key;
            outValue = Value;
        }
    }
}