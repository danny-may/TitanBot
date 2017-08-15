using Discord;
using TitanBot.Commands.Responses;
using TitanBot.Contexts;

namespace TitanBot.Commands
{
    public interface IPermissionManager
    {
        PermissionCheckResponse CheckAllowed(IMessageContext context, CallInfo[] calls);
        CallInfo[] CheckBlacklist(IMessageContext context, CallInfo[] calls);
        CallInfo[] CheckContext(IMessageContext context, CallInfo[] calls);
        CallInfo[] CheckOwner(IMessageContext context, CallInfo[] calls);
        CallInfo[] CheckPermissions(IMessageContext context, CallInfo[] calls);
        void SetPermissions(IMessageContext context, CallInfo[] calls, Optional<ulong?> permission = default(Optional<ulong?>), Optional<ulong[]> roles = default(Optional<ulong[]>), Optional<ulong[]> blacklist = default(Optional<ulong[]>));
        void ResetPermissions(IMessageContext context, CallInfo[] calls);
    }
}
