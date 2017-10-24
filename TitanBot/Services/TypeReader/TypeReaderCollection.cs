using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Core.Services.TypeReader;

namespace TitanBot.Services.TypeReader
{
    internal class TypeReaderCollection : ITypeReaderCollection
    {
        private List<GenericReader> _readerGenerators { get; } = new List<GenericReader>();
        private ConcurrentDictionary<Type, List<ITypeReader>> _readerInstances { get; } = new ConcurrentDictionary<Type, List<ITypeReader>>();

        public TypeReaderCollection()
        {
        }

        public TypeReaderCollection(TypeReaderCollection collection)
        {
            _readerGenerators = new List<GenericReader>(collection._readerGenerators);
            _readerInstances = new ConcurrentDictionary<Type, List<ITypeReader>>(collection._readerInstances);
        }

        public void AddReader<T>(ITypeReader reader)
            => AddReader(reader, typeof(T));

        public void AddReader(GenericReader reader)
            => _readerGenerators.Add(reader);

        public void AddReader(ITypeReader reader, Type type)
        {
            var readers = _readerInstances.GetOrAdd(type, k => new List<ITypeReader>());
            readers.Add(reader);
        }

        public void RemoveReader<T>(ITypeReader reader)
            => RemoveReader(reader, typeof(T));

        public void RemoveReader(GenericReader reader)
            => _readerGenerators.Remove(reader);

        public void RemoveReader(ITypeReader reader, Type type)
        {
            if (_readerInstances.TryGetValue(type, out var readers))
                readers.Remove(reader);
        }

        public bool TryGetReaders(Type type, out List<ITypeReader> readers)
        {
            if (_readerInstances.TryGetValue(type, out readers))
                return true;

            var constructed = _readerGenerators.Select(r => new { Success = r(type, this, out var reader), Readers = reader })
                                              .Where(r => r.Success)
                                              .SelectMany(r => r.Readers)
                                              .ToList();
            if (constructed.Count == 0)
                return false;

            _readerInstances[type] = constructed;

            readers = constructed;
            return true;
        }
    }
}