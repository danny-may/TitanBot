using System.Threading.Tasks;
using TitanBotBase.Commands;

namespace TitanBotBase.TypeReaders
{
    public abstract class TypeReader
    {
        internal abstract Task<TypeReaderResponse> Read(ICommandContext context, string value);
    }
}
