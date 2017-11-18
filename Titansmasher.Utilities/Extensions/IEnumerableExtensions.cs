using System;
using System.Collections.Generic;
using System.Linq;

namespace Titansmasher.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IReadOnlyList<T>> GetPermatations<T>(this IEnumerable<T> source)
        {
            var sourceList = source.ToList();
            var counter = (ulong)Math.Pow(2, sourceList.Count);

            while (counter-- > 0)
            {
                yield return counter.ToBitArray()
                                    .Cast<bool>()
                                    .Take(sourceList.Count)
                                    .Select((b, i) => b ? sourceList[i] : default(T))
                                    .ToList()
                                    .AsReadOnly();
            }
        }
    }
}