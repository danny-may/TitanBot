using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot.TypeReaders
{
    public class TypeReaderCollection : ITypeReaderCollection
    {
        private readonly ConcurrentDictionary<Type, ConcurrentBag<TypeReader>> TypeReaders;
        private readonly ImmutableList<(Type Entity, Type Reader)> EntityReaders;
        private ConcurrentDictionary<(int, Type, string), TypeReaderResponse> ResultsCache = new ConcurrentDictionary<(int, Type, string), TypeReaderResponse>();

        private object _lock = new object();

        private TypeReaderCollection(TypeReaderCollection parent)
        {
            ResultsCache = new ConcurrentDictionary<(int, Type, string), TypeReaderResponse>(parent.ResultsCache);
            TypeReaders = new ConcurrentDictionary<Type, ConcurrentBag<TypeReader>>(parent.TypeReaders);
            EntityReaders = parent.EntityReaders;
        }

        public TypeReaderCollection()
        {
            TypeReaders = new ConcurrentDictionary<Type, ConcurrentBag<TypeReader>>();
            var entityTypeReaders = ImmutableList.CreateBuilder<(Type, Type)>();
            entityTypeReaders.Add((typeof(IMessage), typeof(MessageTypeReader<>)));
            entityTypeReaders.Add((typeof(IChannel), typeof(ChannelTypeReader<>)));
            entityTypeReaders.Add((typeof(IRole), typeof(RoleTypeReader<>)));
            entityTypeReaders.Add((typeof(IUser), typeof(UserTypeReader<>)));
            EntityReaders = entityTypeReaders.ToImmutable();

            foreach (var type in PrimitiveParsers.SupportedTypes)
                AddTypeReader(type, PrimitiveTypeReader.Create(type));

            AddTypeReader<TimeSpan>(new TimeSpanTypeReader());
            AddTypeReader<System.Drawing.Color>(new ColourTypeReader());
        }

        public void AddTypeReader<T>(TypeReader reader)
            => AddTypeReader(typeof(T), reader);
        public void AddTypeReader(Type type, TypeReader reader)
        {
            var readers = TypeReaders.GetOrAdd(type, x => new ConcurrentBag<TypeReader>());
            readers.Add(reader);
        }

        IEnumerable<TypeReader> GetReaders(Type type)
        {
            var knownReaders = TypeReaders.GetOrAdd(type, x => new ConcurrentBag<TypeReader>());
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
            for (int i = 0; i < EntityReaders.Count; i++)
            {
                if (type == EntityReaders[i].Entity || typeInfo.ImplementedInterfaces.Contains(EntityReaders[i].Entity))
                {
                    var reader = Activator.CreateInstance(EntityReaders[i].Reader.MakeGenericType(type)) as TypeReader;
                    AddTypeReader(type, reader);
                }
            }

            return GetReaders(type);
        }

        public async Task<TypeReaderResponse> Read(Type type, ICommandContext context, string text)
        {
            if (ResultsCache.TryGetValue((context.GetHashCode(), type, text), out TypeReaderResponse result))
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
            return ResultsCache.GetOrAdd((context.GetHashCode(), type, text), result);
        }

        public ITypeReaderCollection NewCache()
            => new TypeReaderCollection(this);
    }
}
