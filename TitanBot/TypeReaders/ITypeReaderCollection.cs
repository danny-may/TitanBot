using System;
using System.Threading.Tasks;
using TitanBot.Contexts;

namespace TitanBot.TypeReaders
{
    public interface ITypeReaderCollection
    {
        void AddTypeReader(Type type, TypeReader reader);
        void AddTypeReader<T>(TypeReader reader);
        ITypeReaderCollection NewCache();
        ValueTask<TypeReaderResponse> Read(Type type, IMessageContext context, string text);
    }
}