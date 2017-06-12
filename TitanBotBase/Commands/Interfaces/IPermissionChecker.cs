using System.Collections.Generic;
using TitanBotBase.Commands.Responses;
using TitanBotBase.Database.Tables;

namespace TitanBotBase.Commands
{
    public interface IPermissionChecker
    {
        PermissionCheckResponse CheckAllowed(ICommandContext context, CallInfo[] calls);
        CallInfo[] CheckBlacklist(ICommandContext context, CallInfo[] calls, IEnumerable<CallPermission> permissions);
        CallInfo[] CheckContext(ICommandContext context, CallInfo[] calls);
        CallInfo[] CheckOwner(ICommandContext context, CallInfo[] calls);
        CallInfo[] CheckPermissions(ICommandContext context, CallInfo[] calls, IEnumerable<CallPermission> permissions);
    }
}