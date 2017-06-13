using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBotBase.Commands;

namespace TitanBotBase.TypeReaders
{
    public class TypeReaderCollection : ITypeReaderCollection
    {
        private readonly ConcurrentDictionary<Type, ConcurrentBag<TypeReader>> _typeReaders;
        private readonly ImmutableList<(Type Entity, Type Reader)> _entityReaders;
        private ConcurrentDictionary<(Type, string), TypeReaderResponse> _results = new ConcurrentDictionary<(Type, string), TypeReaderResponse>();

        private object _lock = new object();

        private TypeReaderCollection(TypeReaderCollection parent)
        {
            _results = new ConcurrentDictionary<(Type, string), TypeReaderResponse>(parent._results);
            _typeReaders = new ConcurrentDictionary<Type, ConcurrentBag<TypeReader>>(parent._typeReaders);
            _entityReaders = parent._entityReaders;
        }

        public TypeReaderCollection()
        {
            _typeReaders = new ConcurrentDictionary<Type, ConcurrentBag<TypeReader>>();
            var entityTypeReaders = ImmutableList.CreateBuilder<(Type, Type)>();
            entityTypeReaders.Add((typeof(IMessage), typeof(MessageTypeReader<>)));
            entityTypeReaders.Add((typeof(IChannel), typeof(ChannelTypeReader<>)));
            entityTypeReaders.Add((typeof(IRole), typeof(RoleTypeReader<>)));
            entityTypeReaders.Add((typeof(IUser), typeof(UserTypeReader<>)));
            _entityReaders = entityTypeReaders.ToImmutable();

            foreach (var type in PrimitiveParsers.SupportedTypes)
                AddTypeReader(type, PrimitiveTypeReader.Create(type));
        }

        public void AddTypeReader<T>(TypeReader reader)
            => AddTypeReader(typeof(T), reader);
        public void AddTypeReader(Type type, TypeReader reader)
        {
            var readers = _typeReaders.GetOrAdd(type, x => new ConcurrentBag<TypeReader>());
            readers.Add(reader);
        }

        IEnumerable<TypeReader> GetReaders(Type type)
        {
            var knownReaders = _typeReaders.GetOrAdd(type, x => new ConcurrentBag<TypeReader>());
            return knownReaders;
        }

        IEnumerable<TypeReader> GetTypeReaders(Type type)
        {
            var readers = new List<TypeReader>();
            readers.AddRange(GetCustomTypeReaders(type));

            return new ConcurrentBag<TypeReader>(readers);
        }

        IEnumerable<TypeReader> GetCustomTypeReaders(Type type)
        {
            var readerInstances = GetReaders(type);

            if (readerInstances.Count() > 0)
                return readerInstances;

            var typeInfo = type.GetTypeInfo();
            //Is this an enum?
            if (typeInfo.IsEnum)
            {
                var reader = EnumTypeReader.GetReader(type);
                AddTypeReader(type, reader);
            }

            if (typeInfo.IsArray)
            {
                var arrReaders = ArrayTypeReader.GetReaders(type, GetTypeReaders);
                foreach (var reader in arrReaders)
                    AddTypeReader(type, reader);
            }

            //Is this an entity?
            for (int i = 0; i < _entityReaders.Count; i++)
            {
                if (type == _entityReaders[i].Entity || typeInfo.ImplementedInterfaces.Contains(_entityReaders[i].Entity))
                {
                    var reader = Activator.CreateInstance(_entityReaders[i].Reader.MakeGenericType(type)) as TypeReader;
                    AddTypeReader(type, reader);
                }
            }

            return GetReaders(type);
        }

        public async Task<TypeReaderResponse> Read(Type type, ICommandContext context, string text)
        {
            if (_results.TryGetValue((type, text), out TypeReaderResponse result))
                return result;

            type = Nullable.GetUnderlyingType(type) ?? type;

            var readers = GetTypeReaders(type);

            var resultTasks = readers.Select(r => r.Read(context, text)).ToArray();

            var results = await Task.WhenAll(resultTasks);

            var success = results.Where(r => r.IsSuccess).OrderByDescending(r => r.Values.Max(v => v.Score));

            if (success.Count() > 0)
                result = success.First();

            else if (results.Count() == 1)
                result = results.First();
            else if (results.Count() > 0)
                result = TypeReaderResponse.FromError($"Unable to read the value `{text}` as `{type.Name}`");
            else
                result = TypeReaderResponse.FromError($"No reader found for type `{type.FullName}`");
            return _results.GetOrAdd((type, text), result);
        }

        public ITypeReaderCollection NewCache()
            => new TypeReaderCollection(this);
    }
}
