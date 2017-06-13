using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Commands.Responses;
using TitanBotBase.Database;
using TitanBotBase.Database.Tables;
using TitanBotBase.Util;

namespace TitanBotBase.Commands
{
    public class PermissionChecker : IPermissionChecker
    {
        private IDatabase Database { get; }
        private BotClient BotClient { get; }

        public PermissionChecker(IDatabase database, BotClient botClient)
        {
            Database = database;
            BotClient = botClient;
        }

        public PermissionCheckResponse CheckAllowed(ICommandContext context, CallInfo[] calls)
        {
            var permitted = CheckContext(context, calls);
            if (permitted.Count() == 0)
                return PermissionCheckResponse.FromError("You cannot use that command here!");

            if (BotClient.Owners.Contains(context.Author.Id))
                return PermissionCheckResponse.FromSuccess(calls);

            permitted = CheckOwner(context, permitted);
            if (permitted.Count() == 0)
                return PermissionCheckResponse.FromError("Only owners can use that command!");

            if (context.Channel is IDMChannel || context.Channel is IGroupChannel || context.Guild.OwnerId == context.Author.Id)
                return PermissionCheckResponse.FromSuccess(permitted);

            var guildPermissions = Database.QueryTable((IDbTable<CallPermission> table) => table.Find(p => p.GuildId == context.Guild.Id));

            permitted = CheckBlacklist(context, permitted, guildPermissions);
            if (permitted.Count() == 0)
                return PermissionCheckResponse.FromError();

            permitted = CheckPermissions(context, permitted, guildPermissions);
            if (permitted.Count() == 0)
                return PermissionCheckResponse.FromError("You do not have permission to use that command!");

            return PermissionCheckResponse.FromSuccess(permitted);

        }

        public CallInfo[] CheckContext(ICommandContext context, CallInfo[] calls)
        {
            return calls.Where(c =>
            {
                bool isValid = false;

                if ((c.RequiredContexts & ContextType.Guild) != 0)
                    isValid = isValid || context.Channel is IGuildChannel;
                if ((c.RequiredContexts & ContextType.DM) != 0)
                    isValid = isValid || context.Channel is IDMChannel;
                if ((c.RequiredContexts & ContextType.Group) != 0)
                    isValid = isValid || context.Channel is IGroupChannel;

                return isValid;
            }).ToArray();
        }

        public CallInfo[] CheckOwner(ICommandContext context, CallInfo[] calls)
        {
            var application = context.Client.GetApplicationInfoAsync().Result;
            return calls.Where(c =>
            {
                if (!c.RequireOwner)
                    return true;
                switch (context.Client.TokenType)
                {
                    case TokenType.Bot:
                        if (context.Author.Id != application.Owner.Id)
                            return false;
                        else
                            return true;
                    case TokenType.User:
                        if (context.Author.Id != context.Client.CurrentUser.Id)
                            return false;
                        else
                            return true;
                    default:
                        return false;
                }
            }).ToArray();
        }

        public CallInfo[] CheckBlacklist(ICommandContext context, CallInfo[] calls, IEnumerable<CallPermission> permissions)
        {
            var guildUser = context.Author as IGuildUser;

            if (guildUser.HasAll(context.GuildData.PermOverride))
                return calls;

            if ((context.GuildData.BlackListed?.Length ?? 0) != 0)
                if (context.GuildData.BlackListed.Contains(context.Channel.Id))
                    return new CallInfo[0];

            return calls.Where(c => {
                var permission = permissions.FirstOrDefault(p => p.CallName.ToLower() == c.PermissionKey.ToLower());
                if (permission == null)
                    return true;
                if (permission.Blacklisted != null)
                    return !permission.Blacklisted.Contains(context.Channel.Id);
                return true;
            }).ToArray();
        }

        public CallInfo[] CheckPermissions(ICommandContext context, CallInfo[] calls, IEnumerable<CallPermission> permissions)
        {
            var guildUser = context.Author as IGuildUser;

            return calls.Where(c =>
            {
                var permission = permissions.FirstOrDefault(p => p.CallName.ToLower() == c.PermissionKey.ToLower());
                if (permission == null || (permission.Roles == null || permission.Roles.Count() == 0))
                    return guildUser.HasAll(permission?.Permission ?? c.DefaultPermissions);
                return permission.Roles != null && guildUser.RoleIds.Any(r => permission.Roles.Contains(r));
            }).ToArray();
        }
    }
}
