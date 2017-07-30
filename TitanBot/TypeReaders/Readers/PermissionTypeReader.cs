using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot.TypeReaders
{
    class PermissionTypeReader : TypeReader
    {
        public override ValueTask<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            return ValueTask.FromResult(TypeReaderResponse.FromError(null, null, null));
        }
    }
}
