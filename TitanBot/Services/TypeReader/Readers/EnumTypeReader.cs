using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using TitanBot.Core.Models.Contexts;
using TitanBot.Core.Services.TypeReader;

namespace TitanBot.Services.TypeReader.Readers
{
    internal static class EnumTypeReader
    {
        public static bool TryCreate(Type type, ITypeReaderCollection readerCollection, out IEnumerable<ITypeReader> readers)
        {
            readers = null;
            if (!(type?.IsEnum ?? false))
                return false;

            Type baseType = Enum.GetUnderlyingType(type);
            var constructor = typeof(EnumTypeReader<>).MakeGenericType(baseType).GetTypeInfo().DeclaredConstructors.First();
            if (!readerCollection.TryGetReaders(baseType, out var baseReaders))
                return false;

            readers = baseReaders.Select(r => constructor.Invoke(new object[] { type, r }) as ITypeReader).ToList();

            return true;
        }
    }

    internal class EnumTypeReader<T> : ITypeReader
    {
        private readonly IReadOnlyDictionary<string, object> EnumByName;
        private readonly IReadOnlyDictionary<T, object> EnumByValue;
        private readonly Type EnumType;
        private readonly ITypeReader _reader;

        public EnumTypeReader(Type type, ITypeReader reader)
        {
            EnumType = type;
            _reader = reader;

            var byNameBuilder = ImmutableDictionary.CreateBuilder<string, object>();
            var byValueBuilder = ImmutableDictionary.CreateBuilder<T, object>();

            foreach (var v in Enum.GetNames(EnumType))
            {
                var parsedValue = Enum.Parse(EnumType, v);
                byNameBuilder.Add(v.ToLower(), parsedValue);
                if (!byValueBuilder.ContainsKey((T)parsedValue))
                    byValueBuilder.Add((T)parsedValue, parsedValue);
            }

            EnumByName = byNameBuilder.ToImmutable();
            EnumByValue = byValueBuilder.ToImmutable();
        }

        public ITypeReaderResult Read(IMessageContext context, string text)
        {
            var result = _reader.Read(context, text);

            if (result.IsSuccess)
            {
                if (EnumByValue.TryGetValue((T)result.BestMatch.Value, out object enumValue))
                    return TypeReaderResult.FromSuccess(text, enumValue);
                else
                    return TypeReaderService.UnableToRead(text, EnumType);
            }
            else
            {
                if (EnumByName.TryGetValue(text.ToLower(), out object enumValue))
                    return TypeReaderResult.FromSuccess(text, enumValue);
                else
                    return TypeReaderService.UnableToRead(text, EnumType);
            }
        }
    }
}