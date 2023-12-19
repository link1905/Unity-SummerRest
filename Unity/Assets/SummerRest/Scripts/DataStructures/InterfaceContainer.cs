using System;
using MemoryPack;
using TypeReferences;
using UnityEngine;

namespace SummerRest.DataStructures
{
    [Serializable]
    public abstract class InterfaceContainer<T> : ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private byte[] serializedValue;

        public abstract Type Type { get; }
        [field: SerializeReference, HideInInspector] public T Value { get; private set; }
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
            object deserializedValue;
            try
            {
                if (serializedValue is null || serializedValue.Length == 0)
                    deserializedValue = Activator.CreateInstance(type);
                else
                    deserializedValue = MemoryPackSerializer.Deserialize(type, serializedValue);
            }
            catch (MemoryPackSerializationException serializationException)
            {
                deserializedValue = Activator.CreateInstance(type);
            }
            // var deserializedValue = MemoryPackSerializer.Deserialize(type, serializedValue) ?? Activator.CreateInstance(type);
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
                type ??= Value.GetType();
                serializedValue = MemoryPackSerializer.Serialize(type, Value);
            }
            else
                serializedValue = Array.Empty<byte>();
            valueChanged = false;
            _shouldUpdateValue = true;
        }
    }
}