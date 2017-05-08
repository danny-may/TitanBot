using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public static class ArrayTypeReader
    {
        public static TypeReader GetReader(Type type, Func<Type, TypeReader> readers)
        {
            Type baseType = type.GetElementType();
            var constructor = typeof(ArrayTypeReader<>).MakeGenericType(baseType).GetTypeInfo().DeclaredConstructors.First();
            return (TypeReader)constructor.Invoke(new object[] { type, readers(baseType) });
        }
    }
    public class ArrayTypeReader<T> : TypeReader
    {
        private readonly Type _arrayType;
        private readonly TypeReader _parser;

        public ArrayTypeReader(Type type, TypeReader parser)
        {
            _arrayType = type;
            _parser = parser;
        }

        public override async Task<TypeReaderResult> Read(TitanbotCmdContext context, string value)
        {
            var values = new List<T>();

            if (_parser == null)
                return TypeReaderResult.FromError($"No reader found for `{typeof(T)}`");

            foreach (var item in value.Split(','))
            {
                var response = await _parser?.Read(context, item.Trim());
                if (response.IsSuccess)
                    values.Add((T)response.Best);
                else
                    return TypeReaderResult.FromError($"`{item.Trim()}` is not a valid `{typeof(T).Name}`");
            }

            return TypeReaderResult.FromSuccess(values.ToArray());
        }
    }
}
