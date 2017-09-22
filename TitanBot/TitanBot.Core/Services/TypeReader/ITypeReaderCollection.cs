using System;
using System.Collections.Generic;

namespace TitanBot.Core.Services.TypeReader
{
    public interface ITypeReaderCollection
    {
        bool TryGetReaders(Type type, out List<ITypeReader> readers);

        void AddReader<T>(ITypeReader reader);
        void AddReader(ITypeReader reader, Type type);
        void AddReader(GenericReader reader);
        void RemoveReader<T>(ITypeReader reader);
        void RemoveReader(ITypeReader reader, Type type);
        void RemoveReader(GenericReader reader);
    }
}