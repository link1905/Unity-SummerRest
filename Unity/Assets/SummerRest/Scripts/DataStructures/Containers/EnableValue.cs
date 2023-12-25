using System;
using System.Collections.Generic;
using UnityEngine;

namespace SummerRest.DataStructures.Containers
{
    
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private TKey[] keyData;
        [SerializeField, HideInInspector] private TValue[] valueData;
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < keyData.Length && i < valueData.Length; i++)
                this[keyData[i]] = valueData[i];
        }
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            keyData = new TKey[Count];
            valueData = new TValue[Count];
            var i = 0;
            foreach (var (k, v) in this)
            {
                keyData[i] = k;
                valueData[i] = v;
                i++;
            }
        }
    }
    
    
    [Serializable]
    internal class EnableValue<T>
    {
        [field: SerializeField] public T Value { get; private set; }
        [field: SerializeField] public bool Enable { get; private set; }
    }
}