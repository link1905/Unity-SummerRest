using System.Collections.Generic;

namespace SummerRest.Editor.Utilities
{
    public static class CollectionExtensions
    {
        public static void MatchSize<T>(this IList<T> container, int size)
        {
            if (container.Count < size)
                container.RemoveAt(size);
            else if (container.Count > size)
            {
                var diff = container.Count - size;
                for (var i = 0; i < diff; i++)
                    container.Add(default);
            }
        }
    }
}