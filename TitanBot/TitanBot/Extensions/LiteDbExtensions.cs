using System.Collections.Generic;
using System.Linq;

namespace LiteDB
{
    public static class LiteDbExtensions
    {
        public static BsonArray ToBson<T>(this IEnumerable<T> values)
            => new BsonArray(values.Select(v => new BsonValue(v)));
    }
}