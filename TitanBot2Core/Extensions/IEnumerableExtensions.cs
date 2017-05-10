using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            for (int i = 0; i < Math.Min(data.Count(), maxLength); i++)
            {
                if (i == position)
                    yield return string.Join(" ", data.Skip(i).Take(overflowAmount));
                else
                    yield return data.Skip(i).First();
            }
        }

        public static IEnumerable<T1> TakeMax<T1>(this IEnumerable<T1> value, int count)
        {
            count = Math.Min(count, value.Count());
            return value.Take(count);
        }
    }
}
