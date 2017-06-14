using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBotBase.Commands;

namespace TitanBotBase.TypeReaders
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
        private readonly Type _arrayType;
        private readonly TypeReader _parser;

        public ArrayTypeReader(Type type, TypeReader parser)
        {
            _arrayType = type;
            _parser = parser;
        }

        internal override async Task<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            var values = new List<T>();

            if (_parser == null)
                return TypeReaderResponse.FromError($"No reader found for `{typeof(T)}`");

            if (value == null)
                return TypeReaderResponse.FromSuccess(new T[0]);

            foreach (var item in value.Split(','))
            {
                var response = await _parser?.Read(context, item.Trim());
                if (response.IsSuccess)
                    values.Add((T)response.Best);
                else
                    return TypeReaderResponse.FromError($"`{item.Trim()}` is not a valid `{typeof(T).Name}`");
            }

            return TypeReaderResponse.FromSuccess(values.ToArray());
        }
    }
}
