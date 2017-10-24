using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Core.Models.Contexts;
using TitanBot.Core.Services.TypeReader;

namespace TitanBot.Services.TypeReader
{
    public class TypeReaderResultCache : ITypeReaderResultCache
    {
        public ITypeReaderCollection TypeReaders = new TypeReaderCollection();
        private ReaderCacheStore _cache = new ReaderCacheStore();

        internal TypeReaderResultCache()
        {
        }

        private TypeReaderResultCache(ITypeReaderCollection readers, ReaderCacheStore cache)
        {
            TypeReaders = readers;

            _cache = cache.Clone();
        }

        public ITypeReaderResultCache BeginCache()
            => new TypeReaderResultCache(TypeReaders, _cache);

        public void Dispose()
            => _cache.Clear();

        public ITypeReaderResult Read<T>(IMessageContext context, string text)
            => Read(context, text, typeof(T));

        public ITypeReaderResult Read(IMessageContext context, string text, Type type)
        {
            if (_cache.TryGetValue(text, out var resultCache) && resultCache.TryGetValue(type, out var result))
                return result;

            if (!TypeReaders.TryGetReaders(type, out var readers))
                result = TypeReaderService.MissingReader(type);

            var results = readers.Select(r => r.Read(context, text)).ToArray();
            result = results.OrderByDescending(r => r.BestMatch?.Certainty ?? 0).FirstOrDefault();

            _cache.GetOrAdd(text, k => new TypeResultCache()).TryAdd(type, result);
            return result;
        }

        private class TypeResultCache : ConcurrentDictionary<Type, ITypeReaderResult>
        {
            public TypeResultCache()
            {
            }
            public TypeResultCache(IEnumerable<KeyValuePair<Type, ITypeReaderResult>> collection) : base(collection)
            {
            }

            public TypeResultCache Clone()
                => new TypeResultCache(this);
        }

        private class ReaderCacheStore : ConcurrentDictionary<string, TypeResultCache>
        {
            public ReaderCacheStore()
            {
            }
            public ReaderCacheStore(IEnumerable<KeyValuePair<string, TypeResultCache>> collection) : base(collection)
            {
            }

            public ReaderCacheStore Clone()
                => new ReaderCacheStore(this.Select(k => KeyValuePair.Create(k.Key, k.Value.Clone())));
        }
    }
}