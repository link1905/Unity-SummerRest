using System;

namespace SummerRest.Runtime.DataStructures
{
    /// <summary>
    /// Make an interface always have a default instance
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TDefault"></typeparam>
    public interface IDefaultSupport<TInterface, TDefault> : IDefaultSupport<TInterface> where TDefault : TInterface, new()
    {
        public static TInterface Current { get; set; } = new TDefault();
    }
    public interface IDefaultSupport<TInterface>
    {
    }
}