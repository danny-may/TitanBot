using System.Collections;
using System.Collections.Generic;

namespace System.Linq
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

        public static IEnumerable<string> ToLower(this IEnumerable<string> values)
            => values.Select(v => v.ToLower());
        public static IEnumerable<string> ToUpper(this IEnumerable<string> values)
            => values.Select(v => v.ToUpper());
        public static string[] ToLower(this string[] values)
            => values.Select(v => v.ToLower()).ToArray();
        public static string[] ToUpper(this string[] values)
            => values.Select(v => v.ToUpper()).ToArray();

        public static IEnumerable<bool[]> BooleanMask(int optionCount)
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

        public static (T First, T Second)[] Pair<T>(this T[] source)
        {
            if (source.Length < 2)
                return new(T, T)[0];

            var arr = new(T, T)[source.Length - 1];
            for (int i = 0; i < arr.Length - 1; i++)
                arr[i] = (source[i], source[i + 1]);
            return arr;
        }
    }
}
