using System;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    [Serializable]
    public abstract class InterfaceContainer<T> : ISerializationCallbackReceiver where T : class
    {
        public abstract Type Type { get; }
        
        [SerializeReference] private T value;
        public T Value => value;
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            var type = Type;
            if (type is null)
            {
                value = null;
                return;
            }
            if (value is not null && value.GetType() == type) 
                return;
            value = (T)Activator.CreateInstance(type, true);
        }
    }
}