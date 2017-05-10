using Csv;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TitanBot2.Extensions
{
    public static class ICsvLineExtensions
    {
        private static IEnumerable<string> Enumerate(this ICsvLine line)
            => Enumerable.Range(0, line.ColumnCount).Select(c => line[c]);

        public static IEnumerable<TResult> Select<TResult>(this ICsvLine line, Func<string, TResult> selector)
            => line.Enumerate().Select(selector);

        public static IEnumerable<string> Where(this ICsvLine line, Func<string, bool> predicate)
            => line.Enumerate().Where(predicate);
    }
}
