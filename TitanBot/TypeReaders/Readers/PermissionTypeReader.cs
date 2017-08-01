using System.Threading.Tasks;
using TitanBot.Contexts;

namespace TitanBot.TypeReaders
{
    class PermissionTypeReader : TypeReader
    {
        public override ValueTask<TypeReaderResponse> Read(IMessageContext context, string value)
        {
            return ValueTask.FromResult(TypeReaderResponse.FromError(null, null, null));
        }
    }
}
