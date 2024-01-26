using System;
using SummerRest.Editor.Attributes;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    /// <summary>
    /// Wrap a field which use its own value or inherit from its parent <br/>
    /// Must be used with <see cref="InheritOrCustomAttribute"/>
    /// </summary>
    /// <typeparam name="T">Type of the wrapped field</typeparam>
    [Serializable]
    public class InheritOrCustomContainer<T>
    {
        /// <summary>
        /// Decides the behaviour of this container <seealso cref="InheritChoice"/>
        /// </summary>
        [SerializeField] private InheritChoice inherit = InheritChoice.Inherit;
        /// <summary>
        /// Cached value depended on <see cref="inherit"/>
        /// </summary>
        [SerializeField] private Present<T> cache;
        public Present<T> CacheValue => cache;
        /// <summary>
        /// Underlying value of this container, it may not the same as the returned value of <see cref="Cache{TParent}"/>
        /// </summary>
        [SerializeField] private T value;
        private void Validate(InheritChoice allow, InheritChoice defaultWhenNoParent, object parent)
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
            if (overlap == 0) // Does not overlap => invalid choice
                return defaultWhenNoParent;
            return null;
        }

        /// <summary>
        /// Validate the container and get its value based on <see cref="inherit"/>  
        /// </summary>
        /// <param name="parent">The parent used with {<see cref="whenAppend"/>,<see cref="whenAppend"/>} to remove closures</param>
        /// <param name="whenInherit">Used when <see cref="inherit"/> is <see cref="InheritChoice.Inherit"/></param>
        /// <param name="whenAppend">Used when <see cref="inherit"/> is <see cref="InheritChoice.AppendToParent"/></param>
        /// <param name="whenAuto">Used when <see cref="inherit"/> is <see cref="InheritChoice.Auto"/> </param>
        /// <param name="allow">Move back to <see cref="defaultWhenInvalid"/> when <see cref="inherit"/> are not overlapped with this param</param>
        /// <param name="defaultWhenInvalid"></param>
        /// <typeparam name="TParent">Type of <see cref="parent"/></typeparam>
        /// <returns></returns>
        public Present<T> Cache<TParent>(TParent parent, 
            Func<TParent, Present<T>> whenInherit, 
            Func<TParent, T, Present<T>> whenAppend = null,
            Func<Present<T>> whenAuto = null,
            InheritChoice allow = InheritChoice.None | InheritChoice.Inherit | InheritChoice.Custom, 
            InheritChoice defaultWhenInvalid = InheritChoice.Custom)
        {
            Validate(allow, defaultWhenInvalid, parent);
            switch (inherit)
            {
                case InheritChoice.AppendToParent:
                    if (whenAppend != null) 
                        return cache = whenAppend.Invoke(parent, value);
                    break;
                case InheritChoice.Inherit:
                    if (whenInherit != null) 
                        return cache = whenInherit.Invoke(parent);
                    break;
                case InheritChoice.Custom:
                    return cache = new Present<T>(true, value);
                case InheritChoice.Auto:
                    if (whenAuto != null) 
                        return cache = whenAuto.Invoke();
                    break;
                    
            }
            return new Present<T>(false, value);
        }

    }
}