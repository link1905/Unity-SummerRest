using System;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    public enum TextOrCustomDataType
    {
        PlainText, Data
    }
    [Serializable]
    public class TextOrCustomData<TBody, TBodyContainer> where TBodyContainer : InterfaceContainer<TBody> where TBody : class
    {
        [SerializeField] protected TextOrCustomDataType type;
        [SerializeField] protected string text;
        [SerializeField] protected TBodyContainer body;
        //[SerializeField] private TBodyContainer bodyContainer;
    }
}