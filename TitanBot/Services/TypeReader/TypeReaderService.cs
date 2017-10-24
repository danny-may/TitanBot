using System;
using System.Collections.Generic;
using TitanBot.Core.Services.Formatting.Models;
using TitanBot.Core.Services.TypeReader;
using TitanBot.Services.TypeReader.Readers;

namespace TitanBot.Services.TypeReader
{
    public class TypeReaderService : TypeReaderResultCache, ITypeReaderService
    {
        public static TypeReaderResult MissingReader(Type type)
            => TypeReaderResult.FromError(TransKey.From("TYPEREADER_MISSINGREADER", type));

        public static TypeReaderResult UnableToRead(string text, Type type)
            => TypeReaderResult.FromError(TransKey.From("TYPEREADER_UNABLETOREAD", text, type));

        public TypeReaderService()
        {
            AddReader(PrimitiveTypeReader.TryCreate);
            AddReader(EnumTypeReader.TryCreate);
            AddReader(ArrayTypeReader.TryCreate);
        }

        public bool TryGetReaders(Type type, out List<ITypeReader> readers)
            => TypeReaders.TryGetReaders(type, out readers);

        public void AddReader<T>(ITypeReader reader)
            => TypeReaders.AddReader<T>(reader);

        public void AddReader(ITypeReader reader, Type type)
            => TypeReaders.AddReader(reader, type);

        public void AddReader(GenericReader reader)
            => TypeReaders.AddReader(reader);

        public void RemoveReader<T>(ITypeReader reader)
            => TypeReaders.RemoveReader<T>(reader);

        public void RemoveReader(ITypeReader reader, Type type)
            => TypeReaders.RemoveReader(reader, type);

        public void RemoveReader(GenericReader reader)
            => RemoveReader(reader);
    }
}