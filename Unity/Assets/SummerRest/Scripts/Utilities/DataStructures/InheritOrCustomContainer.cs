using System;
using UnityEngine;

namespace SummerRest.Scripts.Utilities.DataStructures
{
    [Serializable]
    public class InheritOrCustomContainer<T>
    {
        [SerializeField] private InheritChoice inherit = InheritChoice.Inherit;
        public InheritChoice Choice => inherit;
        [SerializeField] private T value;
        public void Validate(InheritChoice allow, InheritChoice defaultWhenNoParent, object parent)
        {
            var backToDefault = ShouldMoveBackToDefault(allow, defaultWhenNoParent, parent);
            if (backToDefault != null)
                inherit = backToDefault.Value;
        }
        private InheritChoice? ShouldMoveBackToDefault(InheritChoice allow, InheritChoice defaultWhenNoParent, object parent)
        {
            if (inherit is InheritChoice.Inherit or InheritChoice.AppendToParent && parent is null)
                return defaultWhenNoParent;
            var overlap = allow & inherit;
            if (overlap == 0) // Does not overlap
                return defaultWhenNoParent;
            return null;
        }
        public T Value => value;
        public T Cache<TParent>(TParent parent, 
            Func<TParent, T> whenInherit, 
            Func<TParent, T, T> whenAppend = null,
            InheritChoice allow = InheritChoice.None | InheritChoice.Inherit | InheritChoice.Custom, 
            InheritChoice defaultWhenNoParent = InheritChoice.Custom)
        {
            Validate(allow, defaultWhenNoParent, parent);
            switch (inherit)
            {
                case InheritChoice.AppendToParent:
                    if (whenAppend != null) 
                        return whenAppend.Invoke(parent, value);
                    break;
                case InheritChoice.Inherit:
                    if (whenInherit != null) 
                        return whenInherit.Invoke(parent);
                    break;
                case InheritChoice.Custom:
                    return value;
            }
            return default;
        }

    }
}