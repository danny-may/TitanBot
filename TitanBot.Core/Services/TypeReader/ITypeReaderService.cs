using System;
using System.Collections.Generic;

namespace TitanBot.Core.Services.TypeReader
{
    public delegate bool GenericReader(Type type, ITypeReaderCollection readerCollection, out IEnumerable<ITypeReader> readers);

    public interface ITypeReaderService : ITypeReaderResultCache, ITypeReaderCollection
    {
    }
}