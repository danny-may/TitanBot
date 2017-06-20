using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot.TypeReaders
{
    static class EnumTypeReader
    {
        public static TypeReader GetReader(Type type)
        {
            Type baseType = Enum.GetUnderlyingType(type);
            var constructor = typeof(EnumTypeReader<>).MakeGenericType(baseType).GetTypeInfo().DeclaredConstructors.First();
            return (TypeReader)constructor.Invoke(new object[] { type, PrimitiveParsers.Get(baseType) });
        }
    }

    class EnumTypeReader<T> : TypeReader
    {
        private readonly IReadOnlyDictionary<string, object> EnumByName;
        private readonly IReadOnlyDictionary<T, object> EnumByValue;
        private readonly Type EnumType;
        private readonly TryParseDelegate<T> TryParse;

        public EnumTypeReader(Type type, TryParseDelegate<T> parser)
        {
            EnumType = type;
            TryParse = parser;

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

        public override Task<TypeReaderResponse> Read(ICommandContext context, string input)
        {
            T baseValue;
            object enumValue;

            if (TryParse(input, out baseValue))
            {
                if (EnumByValue.TryGetValue(baseValue, out enumValue))
                    return Task.FromResult(TypeReaderResponse.FromSuccess(enumValue));
                else
                    return Task.FromResult(TypeReaderResponse.FromError($"Value is not a {EnumType.Name}"));
            }
            else
            {
                if (EnumByName.TryGetValue(input.ToLower(), out enumValue))
                    return Task.FromResult(TypeReaderResponse.FromSuccess(enumValue));
                else
                    return Task.FromResult(TypeReaderResponse.FromError($"Value is not a {EnumType.Name}"));
            }
        }
    }
}
