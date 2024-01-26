using System;
using System.Collections.Generic;
using SummerRest.Runtime.DataStructures;

namespace SummerRest.Editor.DataStructures
{
#if UNITY_EDITOR
    /// <summary>
    /// Editor-only utility class for getting a singleton effectively
    /// </summary>
    public interface IGenericSingleton
    {
        /// <summary>
        /// Caches singletons
        /// </summary>
        private static readonly Dictionary<Type, object> Singletons = new();
        /// <summary>
        /// Get a singleton object by using its type 
        /// </summary>
        /// <param name="type">The type of the singleton</param>
        /// <typeparam name="T">The interface of the singleton</typeparam>
        /// <typeparam name="TDefault">Default value when failed to get or create the singleton</typeparam>
        /// <returns></returns>
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