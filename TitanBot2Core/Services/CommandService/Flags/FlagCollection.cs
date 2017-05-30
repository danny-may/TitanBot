using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Responses;
using TitanBot2.TypeReaders;

namespace TitanBot2.Services.CommandService.Flags
{
    public class FlagCollection : IEnumerable<FlagValue>
    {
        private CmdContext Context { get; }
        private TypeReaderCollection.CachedReader Readers { get; }
        public IReadOnlyCollection<FlagInfo> Flags { get; }
        public IReadOnlyCollection<FlagValue> Values { get; }

        public FlagCollection(CmdContext context, TypeReaderCollection.CachedReader readers, IEnumerable<FlagInfo> flags, IEnumerable<FlagValue> values)
        {
            Context = context;
            Readers = readers;
            Flags = flags.ToList().AsReadOnly();
            Values = values.ToList().AsReadOnly();
        }

        public async Task<TypeReaderResponse> Get(FlagInfo flagInfo)
        {
            var passedValues = FindValues(flagInfo);
            if (passedValues.Count() == 0)
                return TypeReaderResponse.FromError("Flag does not exist");
            if (passedValues.Count() > 1)
                return TypeReaderResponse.FromError("Multiple flags provided");
            if (passedValues.First().Value?.GetType() != typeof(string))
                return TypeReaderResponse.FromError("Invalid flag state");

            return await Readers.Read(flagInfo.FlagType, Context, (string)passedValues.First().Value);
        }

        public bool TryGet<T>(string key, out T value)
        {
            var result = Get(key).Result;

            value = default(T);

            if (!result.IsSuccess)
                return false;
            if (!(result.Best is T))
                return false;
            value = (T)result.Best;
            return true;
        }

        public async Task<TypeReaderResponse> Get(string key)
        {
            var info = FindInfo(key);
            if (info == null)
                return TypeReaderResponse.FromError("Could not find existing flag of that type/key");

            return await Get(info.Value);
        }

        public bool Has(string key)
        {
            var info = FindInfo(key);
            if (info == null)
                return false;

            return FindValues(info.Value).Length == 1;
        }

        private FlagInfo? FindInfo(string key)
            => Flags.Where(f => f.ShortKey.ToLower() == key.ToLower() || f.LongKey.ToLower() == key.ToLower())
                    .Cast<FlagInfo?>()
                    .FirstOrDefault();

        private FlagValue[] FindValues(FlagInfo info)
            => Values.Where(v => v.Key.ToLower() == info.LongKey.ToLower() || v.Key.ToLower() == info.ShortKey.ToLower()).ToArray();

        public IEnumerator<FlagValue> GetEnumerator()
            => Flags.Select(f => new { f, r = Get(f).Result })
                    .Where(a => a.r.IsSuccess)
                    .Select(a => new FlagValue(a.f.ShortKey, a.r.Best))
                    .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
