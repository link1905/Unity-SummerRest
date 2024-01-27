using System;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{

    [Serializable]
    public class TextOrCustomData<TType> where TType : Enum
    {
        [SerializeField] protected TType type;
        public TType Type => type;
        /// <summary>
        /// Raw text value
        /// </summary>
        [SerializeField] protected string text;

        public string Text => text;
    }

    /// <summary>
    /// Used for request body, auth container,... values which maybe raw text or custom class instances
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <typeparam name="TBody">Type of the custom data</typeparam>
    /// <typeparam name="TBodyContainer">Type of custom data container</typeparam>
    [Serializable]
    public class TextOrCustomData<TType, TBody, TBodyContainer> : TextOrCustomData<TType> where TBodyContainer : InterfaceContainer<TBody> where TBody : class where TType : Enum
    {
        /// <summary>
        /// Leverage <see cref="InterfaceContainer{T}"/> to serialize a custom C# class
        /// </summary>
        [SerializeField] protected TBodyContainer body;
    }
}