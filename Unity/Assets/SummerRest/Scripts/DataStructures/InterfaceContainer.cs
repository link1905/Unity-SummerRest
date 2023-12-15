using System;
using MemoryPack;
using SummerRest.Scripts.Attributes;
using SummerRest.Scripts.Models;
using UnityEngine;

namespace SummerRest.Scripts.DataStructures
{



    [Serializable]
    public abstract class InterfaceContainer<T> : ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private byte[] serializedValue;
        //[SerializeField, Inherits(typeof(int), IncludeBaseType = true)] private TypeReference typeRef;
        public Type Type { get; set; }
        [field: SerializeField, HideInInspector] public T Value { get; private set; }
        private bool _shouldUpdateValue = true; 
        [SerializeField, HideInInspector] private bool valueChanged; 
        public TValue GetValue<TValue>() where TValue : class, T
        {
            return Value as TValue;
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            var type = Type;
            if (Value is not null && Value.GetType() == type && !_shouldUpdateValue) 
                return;
            var deserializedValue = MemoryPackSerializer.Deserialize(type, serializedValue) ?? Activator.CreateInstance(type);
            Value = (T)deserializedValue;
            _shouldUpdateValue = false;
        }
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            var type = Type;
            if (!valueChanged && Value is not null && Value.GetType() == type) 
                return;
            if (Value is not null)
            {
                type ??= Type = Value.GetType();
                serializedValue = MemoryPackSerializer.Serialize(type, Value);
            }
            else
                serializedValue = Array.Empty<byte>();
            valueChanged = false;
            _shouldUpdateValue = true;
        }
    }
}