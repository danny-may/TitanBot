using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public static class EnumTypeReader
    {
        public static TypeReader GetReader(Type type)
        {
            Type baseType = Enum.GetUnderlyingType(type);
            var constructor = typeof(EnumTypeReader<>).MakeGenericType(baseType).GetTypeInfo().DeclaredConstructors.First();
            return (TypeReader)constructor.Invoke(new object[] { type, PrimitiveParsers.Get(baseType) });
        }
    }

    public class EnumTypeReader<T> : TypeReader
    {
        private readonly IReadOnlyDictionary<string, object> _enumsByName;
        private readonly IReadOnlyDictionary<T, object> _enumsByValue;
        private readonly Type _enumType;
        private readonly TryParseDelegate<T> _tryParse;

        public EnumTypeReader(Type type, TryParseDelegate<T> parser)
        {
            _enumType = type;
            _tryParse = parser;

            var byNameBuilder = ImmutableDictionary.CreateBuilder<string, object>();
            var byValueBuilder = ImmutableDictionary.CreateBuilder<T, object>();

            foreach (var v in Enum.GetNames(_enumType))
            {
                var parsedValue = Enum.Parse(_enumType, v);
                byNameBuilder.Add(v.ToLower(), parsedValue);
                if (!byValueBuilder.ContainsKey((T)parsedValue))
                    byValueBuilder.Add((T)parsedValue, parsedValue);
            }

            _enumsByName = byNameBuilder.ToImmutable();
            _enumsByValue = byValueBuilder.ToImmutable();
        }

        public override Task<TypeReaderResponse> Read(CmdContext context, string input)
        {
            T baseValue;
            object enumValue;

            if (_tryParse(input, out baseValue))
            {
                if (_enumsByValue.TryGetValue(baseValue, out enumValue))
                    return Task.FromResult(TypeReaderResponse.FromSuccess(enumValue));
                else
                    return Task.FromResult(TypeReaderResponse.FromError($"Value is not a {_enumType.Name}"));
            }
            else
            {
                if (_enumsByName.TryGetValue(input.ToLower(), out enumValue))
                    return Task.FromResult(TypeReaderResponse.FromSuccess(enumValue));
                else
                    return Task.FromResult(TypeReaderResponse.FromError($"Value is not a {_enumType.Name}"));
            }
        }
    }
}
