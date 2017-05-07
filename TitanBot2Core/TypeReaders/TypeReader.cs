using System;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders
{
    public abstract class TypeReader
    {
        public Type Target { get; protected set; }
    }

    public abstract class TypeReader<T> : TypeReader
    {
        public abstract Task<TypeReaderResponse<T>> Read(TitanbotCmdContext context, string value);
    }
}
