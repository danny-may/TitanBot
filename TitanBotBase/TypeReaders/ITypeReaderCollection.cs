using System;
using System.Threading.Tasks;
using TitanBotBase.Commands;

namespace TitanBotBase.TypeReaders
{
    public interface ITypeReaderCollection
    {
        void AddTypeReader(Type type, TypeReader reader);
        void AddTypeReader<T>(TypeReader reader);
        ITypeReaderCollection NewCache();
        Task<TypeReaderResponse> Read(Type type, ICommandContext context, string text);
    }
}