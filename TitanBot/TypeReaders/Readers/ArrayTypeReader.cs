using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot.Contexts;

namespace TitanBot.TypeReaders
{
    static class ArrayTypeReader
    {
        public static IEnumerable<TypeReader> GetReaders(Type type, Func<Type, IEnumerable<TypeReader>> getReaders)
        {
            Type baseType = type.GetElementType();
            var constructor = typeof(ArrayTypeReader<>).MakeGenericType(baseType).GetTypeInfo().DeclaredConstructors.First();
            var readers = getReaders(baseType);
            return readers.Select(r => (TypeReader)constructor.Invoke(new object[] { type, r }));
        }
    }
    class ArrayTypeReader<T> : TypeReader
    {
        private readonly Type ArrayType;
        private readonly TypeReader Parser;

        public ArrayTypeReader(Type type, TypeReader parser)
        {
            ArrayType = type;
            Parser = parser;
        }

        public override async ValueTask<TypeReaderResponse> Read(IMessageContext context, string value)
        {
            var values = new List<T>();

            if (Parser == null)
                return TypeReaderResponse.FromError(TitanBotResource.TYPEREADER_NOTYPEREADER, value, typeof(T));

            if (value == null)
                return TypeReaderResponse.FromSuccess(new T[0]);

            foreach (var item in value.Split(','))
            {
                var response = await Parser.Read(context, item.Trim());
                if (response.IsSuccess)
                    values.Add((T)response.Best);
                else
                    return TypeReaderResponse.FromError(TitanBotResource.TYPEREADER_UNABLETOREAD, item.Trim(), typeof(T));
            }

            return TypeReaderResponse.FromSuccess(values.ToArray());
        }
    }
}
