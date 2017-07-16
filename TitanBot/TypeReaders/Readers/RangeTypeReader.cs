using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot.Commands;
using TitanBot.Models;

namespace TitanBot.TypeReaders
{
    static class RangeTypeReader
    {
        public static IEnumerable<TypeReader> GetReaders(Type type, Func<Type, IEnumerable<TypeReader>> getReaders)
        {
            Type baseType = type.GetGenericArguments().First();
            var constructor = typeof(RangeTypeReader<>).MakeGenericType(baseType).GetTypeInfo().DeclaredConstructors.First();
            var readers = getReaders(baseType);
            return readers.Select(r => (TypeReader)constructor.Invoke(new object[] { type, r }));
        }
    }
    class RangeTypeReader<T> : TypeReader
    {
        private readonly Type RangeType;
        private readonly TypeReader Parser;

        public RangeTypeReader(Type type, TypeReader parser)
        {
            RangeType = type;
            Parser = parser;
        }

        public override async Task<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            if (value == null)
                return TypeReaderResponse.FromSuccess(null);

            var values = value.Split('-');
            if (values.Length == 1)
                values = new string[] { values[0], values[0] };

            if (values.Length != 2)
                return TypeReaderResponse.FromError("TYPEREADER_UNABLETOREAD", value, typeof(Range<T>));

            var from = await Parser.Read(context, values[0]);
            var to = await Parser.Read(context, values[1]);

            if (!from.IsSuccess)
                return TypeReaderResponse.FromError("TYPEREADER_UNABLETOREAD", values[0], typeof(T));
            if (!to.IsSuccess)
                return TypeReaderResponse.FromError("TYPEREADER_UNABLETOREAD", values[1], typeof(T));

            return TypeReaderResponse.FromSuccess(new Range<T>
            {
                From = (T)from.Best,
                To = (T)to.Best
            });
        }
    }
}
