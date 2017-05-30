using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders.Readers;

namespace TitanBot2.TypeReaders
{
    public class TypeReaderCollection
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, TypeReader>> _customReaders;
        private readonly ConcurrentDictionary<Type, TypeReader> _defaultReaders;
        private readonly ImmutableList<Tuple<Type, Type>> _entityReaders;

        private object _lock = new object();

        public TypeReaderCollection()
        {
            _customReaders = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, TypeReader>>();
            _defaultReaders = new ConcurrentDictionary<Type, TypeReader>();
            var entityTypeReaders = ImmutableList.CreateBuilder<Tuple<Type, Type>>();
            entityTypeReaders.Add(new Tuple<Type, Type>(typeof(IMessage), typeof(MessageTypeReader<>)));
            entityTypeReaders.Add(new Tuple<Type, Type>(typeof(IChannel), typeof(ChannelTypeReader<>)));
            entityTypeReaders.Add(new Tuple<Type, Type>(typeof(IRole), typeof(RoleTypeReader<>)));
            entityTypeReaders.Add(new Tuple<Type, Type>(typeof(IUser), typeof(UserTypeReader<>)));
            _entityReaders = entityTypeReaders.ToImmutable();

            foreach (var type in PrimitiveParsers.SupportedTypes)
                _defaultReaders[type] = PrimitiveTypeReader.Create(type);
        }

        public void AddTypeReader<T>(TypeReader reader)
        {
            var readers = _customReaders.GetOrAdd(typeof(T), x => new ConcurrentDictionary<Type, TypeReader>());
            readers[reader.GetType()] = reader;
        }
        public void AddTypeReader(Type type, TypeReader reader)
        {
            var readers = _customReaders.GetOrAdd(type, x => new ConcurrentDictionary<Type, TypeReader>());
            readers[reader.GetType()] = reader;
        }

        private IDictionary<Type, TypeReader> GetTypeReaders(Type type)
        {
            var readers = GetCustomTypeReaders(type) ?? new ConcurrentDictionary<Type, TypeReader>();
            var defaultReader = GetDefaultTypeReader(type);
            if (defaultReader != null && !readers.Keys.Contains(type))
                readers.Add(type, defaultReader);

            return readers;
        }

        IDictionary<Type, TypeReader> GetCustomTypeReaders(Type type)
        {
            ConcurrentDictionary<Type, TypeReader> definedTypeReaders;
            if (_customReaders.TryGetValue(type, out definedTypeReaders))
                return definedTypeReaders;
            return null;
        }

        private TypeReader GetDefaultTypeReader(Type type)
        {
            TypeReader reader;
            if (_defaultReaders.TryGetValue(type, out reader))
                return reader;
            var typeInfo = type.GetTypeInfo();

            //Is this an enum?
            if (typeInfo.IsEnum)
            {
                reader = EnumTypeReader.GetReader(type);
                _defaultReaders[type] = reader;
                return reader;
            }

            if (typeInfo.IsArray)
            {
                reader = ArrayTypeReader.GetReader(type, t => GetTypeReaders(t).Select(r => r.Value).FirstOrDefault());
                _defaultReaders[type] = reader;
                return reader;
            }

            //Is this an entity?
            for (int i = 0; i < _entityReaders.Count; i++)
            {
                if (type == _entityReaders[i].Item1 || typeInfo.ImplementedInterfaces.Contains(_entityReaders[i].Item1))
                {
                    reader = Activator.CreateInstance(_entityReaders[i].Item2.MakeGenericType(type)) as TypeReader;
                    _defaultReaders[type] = reader;
                    return reader;
                }
            }
            return null;
        }

        public async Task<TypeReaderResponse> Read(Type type, CmdContext context, string text)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            var readers = GetTypeReaders(type);

            var resultTasks = readers.Select(r => r.Value.Read(context, text)).ToArray();

            var results = await Task.WhenAll(resultTasks);

            var success = results.Where(r => r.IsSuccess).OrderByDescending(r => r.Values.Max(v => v.Score));

            if (success.Count() > 0)
                return success.First();

            if (results.Count() == 1)
                return results.First();
            if (results.Count() > 0)
                return TypeReaderResponse.FromError($"Unable to read the value `{text}` as `{type.Name}`");

            return TypeReaderResponse.FromError($"No reader found for type `{type.FullName}`");
        }

        public CachedReader NewCache()
        {
            return new CachedReader(this);
        }

        public class CachedReader
        {
            private ConcurrentDictionary<ValueTuple<Type, string>, TypeReaderResponse> _results  = new ConcurrentDictionary<ValueTuple<Type, string>, TypeReaderResponse>();
            private TypeReaderCollection _parent;

            internal CachedReader(TypeReaderCollection parent)
            {
                _parent = parent;
            }

            public async Task<TypeReaderResponse> Read(Type type, CmdContext context, string text)
            {
                var key = ValueTuple.Create(type, text);
                if (_results.TryGetValue(key, out TypeReaderResponse result))
                    return result;

                result = await _parent.Read(type, context, text);
                return _results.GetOrAdd(key, result);
            }
        }
    }
}
