using System;
using System.Collections.Generic;

namespace SummerRest.Scripts.Utilities.DataStructures
{
    public class SimpleObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public event Action<TKey> OnChanged;

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                base[key] = value;
                OnChanged?.Invoke(key);
            }
        }

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            OnChanged?.Invoke(key);
        }
        public new bool Remove(TKey key)
        {
            var rev = base.Remove(key);
            if (rev)
                OnChanged?.Invoke(key);
            return rev;
        }
    }
}