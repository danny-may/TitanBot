using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot.TypeReaders
{
    class PermissionTypeReader : TypeReader
    {
        public override Task<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            return Task.FromResult(TypeReaderResponse.FromError(null, null, null));
        }
    }
}
