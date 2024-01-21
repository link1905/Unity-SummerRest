using System;
using SummerRest.Editor.TypeReference;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    /// <summary>
    /// Based type for serializing interfaces inside Unity Inspector
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class InterfaceContainer<T> : ISerializationCallbackReceiver where T : class
    {
        /// <summary>
        /// Type of the instance often resolved from a <see cref="ClassTypeReference"/> field
        /// </summary>
        public abstract Type Type { get; }
        /// <summary>
        /// Serialized value of the instance supported by Unity serializing system
        /// </summary>
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