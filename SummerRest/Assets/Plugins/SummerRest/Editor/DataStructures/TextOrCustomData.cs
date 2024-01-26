using System;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    public enum TextOrCustomDataType
    {
        PlainText, Data
    }
    /// <summary>
    /// Used for request body, auth container,... values which maybe raw text or custom class instances
    /// </summary>
    /// <typeparam name="TBody">Type of the custom data</typeparam>
    /// <typeparam name="TBodyContainer">Type of custom data container</typeparam>
    [Serializable]
    public class TextOrCustomData<TBody, TBodyContainer> where TBodyContainer : InterfaceContainer<TBody> where TBody : class
    {
        [SerializeField] protected TextOrCustomDataType type;
        /// <summary>
        /// Raw text value
        /// </summary>
        [SerializeField] protected string text;
        /// <summary>
        /// Leverage <see cref="InterfaceContainer{T}"/> to serialize a custom C# class
        /// </summary>
        [SerializeField] protected TBodyContainer body;
    }
}