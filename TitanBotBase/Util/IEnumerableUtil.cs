using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TitanBotBase.Util
{
    public static class IEnumerableUtil
    {
        public static IEnumerable<TResult> ForEach<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> action)
        {
            foreach (var item in source)
            {
                yield return action(item);
            }
        }
        
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var index = 0;
            foreach (var item in source)
            {
                action(item, index);
                index++;
            }
        }

        public static IEnumerable<bool[]> BinomialMask(int optionCount)
        {
            var counter = (int)Math.Pow(2, optionCount);
            while (counter-- > 0)
            {
                yield return new BitArray(new int[] { counter })
                                    .Cast<bool>()
                                    .Take(optionCount)
                                    .Reverse()
                                    .ToArray();
            }
        }
    }
}
