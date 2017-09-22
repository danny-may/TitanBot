using System;
using TitanBot.Core.Models.Contexts;
using TitanBot.Core.Services.TypeReader;

namespace TitanBot.Services.TypeReader.Readers
{
    internal class EntityTypeReader
    {
        //public static bool TryCreate(Type type, ITypeReaderCollection readerCollection, out IEnumerable<ITypeReader> readers)
        //{
        //}
        //
        //public static Dictionary<Type, string> InterfaceMap = new Dictionary<Type, string>
        //{
        //}
        //
        //public static [] TagFormat<T>()
        //{
        //    var interfaces = typeof(T).GetInterfaces();
        //    if ()
        //}
    }

    internal class EntityTypeReader<T> : ITypeReader
    {
        public ITypeReaderResult Read(IMessageContext context, string text)
        {
            throw new NotImplementedException();
        }
    }
}