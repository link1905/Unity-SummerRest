using System;
using System.Collections.Generic;
using SummerRest.Runtime.DataStructures;

namespace SummerRest.Editor.DataStructures
{
#if UNITY_EDITOR
    public interface IGenericSingleton
    {
        private static readonly Dictionary<Type, object> Singletons = new();
        public static T GetSingleton<T, TDefault>(Type type) where T : class where TDefault : class, T, ISingleton<TDefault>, new()
        {
            if (type is null)
                return null;
            T result = null;
            if (!Singletons.TryGetValue(type, out var singleton))
            {
                try
                {
                    singleton = Activator.CreateInstance(type);
                    Singletons.Add(type, singleton);
                }
                catch (Exception)
                {
                    result = null;
                }
            }
            result = singleton as T ?? ISingleton<TDefault>.GetSingleton();
            return result;
        }
    }
#endif
}