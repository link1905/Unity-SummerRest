using System;
using SummerRest.DataStructures.Enums;
using UnityEngine;

namespace SummerRest.DataStructures.Containers
{
    [Serializable]
    public partial class InheritOrCustomContainer<T>
    {
        [SerializeField] private InheritChoice inherit = InheritChoice.Inherit;
        [SerializeField] private T value;
        private T _cache;
        public T? Value => inherit == InheritChoice.None ? default : _cache;
        public void Cache(Func<T> whenInherit = null, Func<T, T> whenAppend = null)
        {
            switch (inherit)
            {
                case InheritChoice.Inherit:
                    if (whenInherit != null) 
                        _cache = whenInherit.Invoke();
                    break;
                case InheritChoice.AppendToParent:
                    if (whenAppend != null) 
                        _cache = whenAppend.Invoke(value);
                    break;
                case InheritChoice.Custom:
                    _cache = value;
                    break;
            }
        }
    }
}