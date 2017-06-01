using System;
using System.Collections.Generic;
using System.Linq;

namespace TitanBot2.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<Tuple<T1, T2>> MemberwisePair<T1, T2>(this IEnumerable<T1> parent, IEnumerable<T2> child, bool allowNullMatch = false)
        {
            int length;
            if (allowNullMatch)
                length = Math.Max(parent.Count(), child.Count());
            else
                length = Math.Min(parent.Count(), child.Count());

            for (int i = 0; i < length; i++)
            {
                yield return new Tuple<T1, T2>(parent.Count() > i ? parent.Skip(i).First() : default(T1),
                                               child.Count() > i ? child.Skip(i).First() : default(T2));
            }
        }

        public static IEnumerable<string> Squeeze(this IEnumerable<string> data, int maxLength, int position = -1)
        {
            if (maxLength == 0)
                yield break;

            position = (maxLength + position) % maxLength;
            var overflowAmount = Math.Max(1, data.Count() - maxLength +1);
            for (int i = 0; i < data.Count(); i++)
            {
                if (i == position)
                {
                    var res = string.Join(" ", data.Skip(i).Take(overflowAmount));
                    i += overflowAmount - 1;
                    yield return res;
                }
                else
                    yield return data.Skip(i).First();
            }
        }

        public static IEnumerable<T1> TakeMax<T1>(this IEnumerable<T1> value, int count)
        {
            count = Math.Min(count, value.Count());
            return value.Take(count);
        }

        public static bool Exists<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => source.Where(predicate).Count() > 0;

        public static IEnumerable<TValue> SelectDistinct<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> selector)
            => source.Select(selector).Distinct();

        public static IEnumerable<TSource> FirstFor<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
