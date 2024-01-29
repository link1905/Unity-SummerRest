using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    /// <summary>
    /// Alternate for non-serialized built-in <see cref="KeyValuePair"/>
    /// </summary>
    [Serializable]
    public struct KeyValue
    {
        [SerializeField] private string key;
        [SerializeField] private string value;
        
        
        [XmlAttribute]
        public string Key
        {
            get => key;
            set => key = value;
        }
        [XmlAttribute]
        public string Value
        {
            get => value;
            set => this.value = value;
        }
        public KeyValue(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
        public KeyValue Clone()
        {
            return new KeyValue(key, value);
        }

        public static implicit operator KeyValuePair<string, string>(KeyValue key) => new(key.Key, key.Value);
        public static implicit operator KeyValue(KeyValuePair<string, string> key) => new(key.Key, key.Value);
 
        public void Deconstruct(out string outKey, out string outValue)
        {
            outKey = Key;
            outValue = Value;
        }
    }
}