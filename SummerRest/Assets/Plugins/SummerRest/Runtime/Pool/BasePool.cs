using UnityEngine.Pool;

namespace SummerRest.Runtime.Pool
{
    /// <summary>
    /// Unity pool wrapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TData"></typeparam>
    internal static class BasePool<T, TData> where T : class, IPoolable<T, TData>, new()
    {
        private static IObjectPool<T> Pool { get; }
        static BasePool()
        {
            Pool = new ObjectPool<T>(() => new T());
        }
        public static int CountInactive => Pool.CountInactive;
        public static void Clear() => Pool.Clear();
        public static T Create(TData initData)
        {
            var item = Pool.Get();
            item.Init(initData);
            item.Pool = Pool;
            return item;
        }
    }
}