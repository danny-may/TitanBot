using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using TitanBotBase.Commands;

namespace TitanBotBase.TypeReaders
{
    static class PrimitiveTypeReader
    {
        public static TypeReader Create(Type type)
        {
            type = typeof(PrimitiveTypeReader<>).MakeGenericType(type);
            return Activator.CreateInstance(type) as TypeReader;
        }
    }

    class PrimitiveTypeReader<T> : TypeReader
    {
        private readonly TryParseDelegate<T> _tryParse;

        public PrimitiveTypeReader()
        {
            _tryParse = PrimitiveParsers.Get<T>();
        }

        internal override Task<TypeReaderResponse> Read(ICommandContext context, string input)
        {
            if (_tryParse(input, out T value))
                return Task.FromResult(TypeReaderResponse.FromSuccess(value));
            return Task.FromResult(TypeReaderResponse.FromError($"Failed to parse {typeof(T).Name}"));
        }
    }

    public delegate bool TryParseDelegate<T>(string str, out T value);

    public static class PrimitiveParsers
    {
        private static readonly Lazy<IReadOnlyDictionary<Type, Delegate>> _parsers = new Lazy<IReadOnlyDictionary<Type, Delegate>>(CreateParsers);

        public static IEnumerable<Type> SupportedTypes = _parsers.Value.Keys;

        static IReadOnlyDictionary<Type, Delegate> CreateParsers()
        {
            var parserBuilder = ImmutableDictionary.CreateBuilder<Type, Delegate>();
            parserBuilder[typeof(sbyte)] = (TryParseDelegate<sbyte>)sbyte.TryParse;
            parserBuilder[typeof(byte)] = (TryParseDelegate<byte>)byte.TryParse;
            parserBuilder[typeof(short)] = (TryParseDelegate<short>)short.TryParse;
            parserBuilder[typeof(ushort)] = (TryParseDelegate<ushort>)ushort.TryParse;
            parserBuilder[typeof(int)] = (TryParseDelegate<int>)int.TryParse;
            parserBuilder[typeof(uint)] = (TryParseDelegate<uint>)uint.TryParse;
            parserBuilder[typeof(long)] = (TryParseDelegate<long>)long.TryParse;
            parserBuilder[typeof(ulong)] = (TryParseDelegate<ulong>)ulong.TryParse;
            parserBuilder[typeof(float)] = (TryParseDelegate<float>)float.TryParse;
            parserBuilder[typeof(double)] = (TryParseDelegate<double>)double.TryParse;
            parserBuilder[typeof(decimal)] = (TryParseDelegate<decimal>)decimal.TryParse;
            parserBuilder[typeof(DateTime)] = (TryParseDelegate<DateTime>)DateTime.TryParse;
            parserBuilder[typeof(DateTimeOffset)] = (TryParseDelegate<DateTimeOffset>)DateTimeOffset.TryParse;
            parserBuilder[typeof(TimeSpan)] = (TryParseDelegate<TimeSpan>)TimeSpan.TryParse;
            parserBuilder[typeof(char)] = (TryParseDelegate<char>)char.TryParse;
            parserBuilder[typeof(bool)] = (TryParseDelegate<bool>)delegate (string str, out bool value)
            {
                switch (str.ToLower())
                {
                    case "t":
                    case "true":
                    case "y":
                    case "yes":
                        value = true;
                        return true;
                    case "f":
                    case "false":
                    case "n":
                    case "no":
                        value = false;
                        return true;
                    default:
                        value = false;
                        return false;
                }
            };
            parserBuilder[typeof(string)] = (TryParseDelegate<string>)delegate (string str, out string value)
            {
                value = str;
                return true;
            };
            parserBuilder[typeof(Uri)] = (TryParseDelegate<Uri>)delegate (string str, out Uri value)
            {
                return Uri.TryCreate(str.Trim('<', '>', ' '), UriKind.Absolute, out value);
            };
            return parserBuilder.ToImmutable();
        }

        public static TryParseDelegate<T> Get<T>() => (TryParseDelegate<T>)_parsers.Value[typeof(T)];
        public static Delegate Get(Type type) => _parsers.Value[type];
    }
}
