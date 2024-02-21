using System;
using System.Collections.Generic;
using System.Linq;

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

        public static TKey[] FindDuplicates<T, TKey>(this IEnumerable<T> container, Func<T, TKey> extractor)
        {
            return container.GroupBy(extractor).Where(g => g.Count() > 1).Select(e => e.Key).ToArray();
        }
    }
}