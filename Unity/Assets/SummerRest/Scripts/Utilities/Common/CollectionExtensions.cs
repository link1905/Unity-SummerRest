using System.Collections.Generic;

namespace SummerRest.Scripts.Utilities.Common
{
    public static class CollectionExtensions
    {
        public static void ClearThenAdd<T>(this List<T> list, IEnumerable<T> add)
        {
            list.Clear();
            list.AddRange(add);
        }
    }
}