using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TitanBot.Core.Models.Contexts;
using TitanBot.Core.Services.TypeReader;

namespace TitanBot.Services.TypeReader.Readers
{
    internal static class ArrayTypeReader
    {
        public static bool TryCreate(Type type, ITypeReaderCollection readerCollection, out IEnumerable<ITypeReader> readers)
        {
            readers = null;

            if (!type.IsArray)
                return false;

            Type baseType = type.GetElementType();
            var constructor = typeof(ArrayTypeReader<>).MakeGenericType(baseType).GetTypeInfo().DeclaredConstructors.First();
            if (!readerCollection.TryGetReaders(baseType, out var baseReaders))
                return false;

            readers = baseReaders.Select(r => constructor.Invoke(new object[] { type, r }) as ITypeReader);
            return true;
        }
    }

    internal class ArrayTypeReader<T> : ITypeReader
    {
        private readonly ITypeReader Parser;
        private readonly Type ArrayType;

        public ArrayTypeReader(Type arrayType, ITypeReader parser)
        {
            ArrayType = arrayType;
            Parser = parser;
        }

        public ITypeReaderResult Read(IMessageContext context, string text)
        {
            var values = new List<ITypeReaderMatch>();

            if (Parser == null)
                return TypeReaderService.MissingReader(typeof(T));

            if (text == null)
                return TypeReaderResult.FromSuccess(text, new T[0]);

            foreach (var item in text.Split(','))
            {
                var response = Parser.Read(context, item.Trim());
                if (response.IsSuccess)
                    values.Add(response.BestMatch);
                else
                    return TypeReaderService.UnableToRead(text, typeof(T));
            }

            return TypeReaderResult.FromSuccess(text, new TypeReaderMatch(values.Min(v => v.Certainty), values.Select(v => v.Value).Cast<T>().ToArray()));
        }
    }
}