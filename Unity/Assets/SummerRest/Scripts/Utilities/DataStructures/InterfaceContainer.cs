using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SummerRest.Scripts.Utilities.DataStructures
{
    [Serializable]
    public abstract class InterfaceContainer<T> : ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private string serializedValue;
        public string SerializedData => serializedValue; 
        public abstract Type Type { get; }
        public T Value
        {
            get => value;
            private set => this.value = value;
        }

        private bool _shouldUpdateValue = true; 
        [SerializeField, HideInInspector] private bool valueChanged;
        [SerializeReference, HideInInspector] private T value;

        public TValue GetValue<TValue>() where TValue : class, T
        {
            return Value as TValue;
        }
        public void OnAfterDeserialize()
        {
            var type = Type;
            if (type is null)
                return;
            if (Value is not null && Value.GetType() == type && !_shouldUpdateValue) 
                return;
            object deserializedValue;
            try
            {
                if (serializedValue is null || serializedValue.Length == 0)
                    deserializedValue = Activator.CreateInstance(type);
                else
                {
                    deserializedValue = JsonConvert.DeserializeObject(serializedValue, type);
                    deserializedValue ??= Activator.CreateInstance(type);
                }
            }
            catch (JsonSerializationException)
            {
                deserializedValue = Activator.CreateInstance(type);
            }
            Value = (T)deserializedValue;
            _shouldUpdateValue = false;
        }
        public void OnBeforeSerialize()
        {
            var type = Type;
            if (type is null)
                return;
            if ( !valueChanged && Value is not null && Value.GetType() == type) 
                return;
            if (Value is not null)
            {
                type ??= Value.GetType();
                try
                {
                    if (type == Value.GetType())
                        serializedValue = JsonConvert.SerializeObject(Value);
                    else
                        serializedValue = string.Empty;
                }
                catch (JsonSerializationException)
                {
                    serializedValue = string.Empty;
                }
            }
            else
                serializedValue = string.Empty;
            valueChanged = false;
            _shouldUpdateValue = true;
        }
    }
}