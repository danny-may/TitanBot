using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot.TypeReaders
{
    public abstract class TypeReader
    {
        public abstract Task<TypeReaderResponse> Read(ICommandContext context, string value);
    }
}
