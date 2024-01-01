using System;
using UnityEngine;

namespace SummerRest.Scripts.Utilities.DataStructures
{
    [Serializable]
    public class InheritOrCustomContainer<T>
    {
        [SerializeField] private InheritChoice inherit = InheritChoice.Inherit;
        [SerializeField] private T value;
        public T Cache(Func<T> whenInherit = null, Func<T, T> whenAppend = null)
        {
            switch (inherit)
            {
                case InheritChoice.Inherit:
                    if (whenInherit != null) 
                        return whenInherit.Invoke();
                    break;
                case InheritChoice.AppendToParent:
                    if (whenAppend != null) 
                        return whenAppend.Invoke(value);
                    break;
                case InheritChoice.Custom:
                    return value;
            }
            return default;
        }
    }
}