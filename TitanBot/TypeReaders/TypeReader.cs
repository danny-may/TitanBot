using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot.TypeReaders
{
    public abstract class TypeReader
    {
        public abstract ValueTask<TypeReaderResponse> Read(ICommandContext context, string value);
    }
}
