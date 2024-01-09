using System;
using System.Collections.Generic;

namespace SummerRest.Utilities.DataStructures
{
    public interface ISingleton<TType> where TType : class, ISingleton<TType>, new()
    {
        private static TType _singleton;
        public static TType GetSingleton()
        {
            return _singleton ??= new TType();
        }
    }

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
                catch (Exception e)
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