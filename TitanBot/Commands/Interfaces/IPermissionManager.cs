using TitanBot.Commands.Responses;

namespace TitanBot.Commands
{
    public interface IPermissionManager
    {
        PermissionCheckResponse CheckAllowed(ICommandContext context, CallInfo[] calls);
        CallInfo[] CheckBlacklist(ICommandContext context, CallInfo[] calls);
        CallInfo[] CheckContext(ICommandContext context, CallInfo[] calls);
        CallInfo[] CheckOwner(ICommandContext context, CallInfo[] calls);
        CallInfo[] CheckPermissions(ICommandContext context, CallInfo[] calls);
        void SetPermissions(ICommandContext context, CallInfo[] calls, ulong? permission, ulong[] roles, ulong[] blacklist);
        void ResetPermissions(ICommandContext context, CallInfo[] calls);
    }
}
