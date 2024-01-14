using System;
using UnityEngine.Pool;

namespace SummerRest.Runtime.Pool
{
    internal interface IPoolable<T, in TInitData> : IPoolable<T> where T : class, new()
    {
        void Init(TInitData data);
    }

    internal interface IPoolable<T> : IDisposable where T : class, new()
    {
        IObjectPool<T> Pool { set; }
    }
}