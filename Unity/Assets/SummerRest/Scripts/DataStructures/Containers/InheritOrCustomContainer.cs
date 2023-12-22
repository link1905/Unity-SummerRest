using System;
using SummerRest.DataStructures.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace SummerRest.DataStructures.Containers
{
    [Serializable]
    public class InheritOrCustomContainer<T>
    {
        [SerializeField] private InheritChoice inherit = InheritChoice.Inherit;
        [SerializeField] private T value;
        [SerializeField] private T cache;
        public T? Value => inherit == InheritChoice.None ? default : cache;
        public void Cache(Func<T> whenInherit = null, Func<T, T> whenAppend = null)
        {
            switch (inherit)
            {
                case InheritChoice.Inherit:
                    if (whenInherit != null) 
                        cache = whenInherit.Invoke();
                    break;
                case InheritChoice.AppendToParent:
                    if (whenAppend != null) 
                        cache = whenAppend.Invoke(value);
                    break;
                case InheritChoice.Custom:
                    cache = value;
                    break;
            }
        }
    }
}