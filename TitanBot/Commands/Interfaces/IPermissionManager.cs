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
        void SetPermissions(IMessageContext context, CallInfo[] calls, ulong? permission, ulong[] roles, ulong[] blacklist);
        void ResetPermissions(IMessageContext context, CallInfo[] calls);
    }
}
