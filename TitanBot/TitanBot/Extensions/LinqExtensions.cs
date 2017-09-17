using System.Collections.Generic;

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static int IndexOf<T>(this IList<T> collection, Predicate<T> predicate)
        {
            for (int i = 0; i < collection.Count; i++)
                if (predicate(collection[i]))
                    return i;
            return -1;
        }
    }
}