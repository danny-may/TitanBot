using System.Threading.Tasks;
using TitanBot.Contexts;

namespace TitanBot.TypeReaders
{
    public abstract class TypeReader
    {
        public abstract ValueTask<TypeReaderResponse> Read(IMessageContext context, string value);
    }
}
