using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot.Contexts;

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

        public override ValueTask<TypeReaderResponse> Read(IMessageContext context, string input)
        {
            if (TryParse(input, out T baseValue))
            {
                if (EnumByValue.TryGetValue(baseValue, out object enumValue))
                    return ValueTask.FromResult(TypeReaderResponse.FromSuccess(enumValue));
                else
                    return ValueTask.FromResult(TypeReaderResponse.FromError(TitanBotResource.TYPEREADER_UNABLETOREAD, input, typeof(T)));
            }
            else
            {
                if (EnumByName.TryGetValue(input.ToLower(), out object enumValue))
                    return ValueTask.FromResult(TypeReaderResponse.FromSuccess(enumValue));
                else
                    return ValueTask.FromResult(TypeReaderResponse.FromError(TitanBotResource.TYPEREADER_UNABLETOREAD, input, typeof(T)));
            }
        }
    }
}
