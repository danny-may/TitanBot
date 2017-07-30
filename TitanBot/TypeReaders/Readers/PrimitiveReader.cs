using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot.TypeReaders
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

        public override ValueTask<TypeReaderResponse> Read(ICommandContext context, string text)
        {
            if (_tryParse(text, out T value))
                return ValueTask.FromResult(TypeReaderResponse.FromSuccess(value));
            return ValueTask.FromResult(TypeReaderResponse.FromError("TYPEREADER_UNABLETOREAD", text, typeof(T)));
        }
    }

    public delegate bool TryParseDelegate<T>(string str, out T value);

    public static class PrimitiveParsers
    {
        private static readonly Lazy<IReadOnlyDictionary<Type, Delegate>> Parsers = new Lazy<IReadOnlyDictionary<Type, Delegate>>(CreateParsers);

        public static IEnumerable<Type> SupportedTypes = Parsers.Value.Keys;

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

        public static TryParseDelegate<T> Get<T>() => (TryParseDelegate<T>)Parsers.Value[typeof(T)];
        public static Delegate Get(Type type) => Parsers.Value[type];
    }
}
