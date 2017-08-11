using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Commands.Models;
using TitanBot.Commands.Responses;
using TitanBot.Contexts;
using TitanBot.Models;
using TitanBot.Settings;
using TitanBot.Storage;
using static TitanBot.TBLocalisation.Logic;

namespace TitanBot.Commands
{
    class PermissionManager : IPermissionManager
    {
        private IDatabase Database { get; }
        private BotClient BotClient { get; }
        private ISettingManager Settings { get; }
        private CachedDictionary<ulong?, Dictionary<string, CallPermission>> CachedPermissions { get; }

        public PermissionManager(IDatabase database, BotClient botClient, ISettingManager settings)
        {
            Database = database;
            BotClient = botClient;
            Settings = settings;
            CachedPermissions = CachedDictionary.FromSource((ulong? key) => LoadPerms(key), new TimeSpan(0, 30, 0), 100);
        }

        private async ValueTask<Dictionary<string, CallPermission>> LoadPerms(ulong? guildId)
            => (await Database.Find<CallPermission>(r => r.GuildId == guildId)).GroupBy(s => s.CallName).ToDictionary(s => s.Key, s => s.First());

        public PermissionCheckResponse CheckAllowed(IMessageContext context, CallInfo[] calls)
        {
            var permitted = CheckContext(context, calls);
            if (permitted.Count() == 0)
                return PermissionCheckResponse.FromError(PERMISSIONMANAGER_DISALLOWED_NOTHERE);

            if (BotClient.Owners.Contains(context.Author.Id))
                return PermissionCheckResponse.FromSuccess(permitted);

            permitted = CheckOwner(context, permitted);
            if (permitted.Count() == 0)
                return PermissionCheckResponse.FromError(PERMISSIONMANAGER_DISALLOWED_NOTOWNER);

            if (context.Channel is IDMChannel || context.Channel is IGroupChannel || context.Guild.OwnerId == context.Author.Id)
                return PermissionCheckResponse.FromSuccess(permitted);

            var settings = Settings.GetContext(context.Guild).Get<GeneralGuildSetting>();
            if (DoesOverride(context, settings))
                return PermissionCheckResponse.FromSuccess(calls);

            permitted = CheckBlacklist(context, permitted);
            if (permitted.Count() == 0)
                return PermissionCheckResponse.FromError();

            permitted = CheckPermissions(context, permitted);
            if (permitted.Count() == 0)
                return PermissionCheckResponse.FromError(PERMISSIONMANAGER_DISALLOWED_NOPERMISSION);

            return PermissionCheckResponse.FromSuccess(permitted);

        }

        private bool DoesOverride(IMessageContext context, GeneralGuildSetting settings)
        {
            if (settings.RoleOverride != null && ((context.Author as IGuildUser)?.RoleIds.Any(r => settings.RoleOverride.Contains(r)) ?? false))
                return true;
            if ((context.Author as IGuildUser)?.HasAll(settings.PermOverride) ?? false)
                return true;
            return false;
        }

        public CallInfo[] CheckContext(IMessageContext context, CallInfo[] calls)
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

                return isValid && (c.Parent.RequireGuild == null ||
                                   c.Parent.RequireGuild.Length == 0 ||
                                   (context.Guild != null && c.Parent.RequireGuild.Contains(context.Guild.Id)));
            }).ToArray();
        }

        public CallInfo[] CheckOwner(IMessageContext context, CallInfo[] calls)
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

        public CallInfo[] CheckBlacklist(IMessageContext context, CallInfo[] calls)
        {
            var guildUser = context.Author as IGuildUser;

            if (guildUser.HasAll(context.GeneralGuildSetting.PermOverride))
                return calls;

            if ((context.GeneralGuildSetting.BlackListed?.Length ?? 0) != 0)
                if (context.GeneralGuildSetting.BlackListed.Contains(context.Channel.Id))
                    return new CallInfo[0];

            var callPerms = GetPerms(context.Guild?.Id, calls);

            return callPerms.Where(p => p.Value == null ||
                                        p.Value.Blacklisted == null ||
                                        !p.Value.Blacklisted.Contains(context.Channel.Id))
                            .Select(p => p.Key)
                            .ToArray();
        }

        public CallInfo[] CheckPermissions(IMessageContext context, CallInfo[] calls)
        {
            var guildUser = context.Author as IGuildUser;

            var callPerms = GetPerms(context.Guild?.Id, calls);

            return callPerms.Where(p =>
            {
                var hasPerm = p.Value?.Permission != null;
                var hasRoles = (p.Value?.Roles?.Length ?? 0) != 0;
                if (p.Value == null || (!hasPerm && !hasRoles))
                    return guildUser.HasAll(p.Key.DefaultPermissions);
                if (hasPerm && guildUser.HasAll(p.Value.Permission.Value))
                    return true;
                if (hasRoles && guildUser.RoleIds.Any(r => p.Value.Roles.Contains(r)))
                    return true;
                return false;
            }).Select(p => p.Key).ToArray();
        }

        public async void SetPermissions(IMessageContext context, CallInfo[] calls, Optional<ulong?> permId, Optional<ulong[]> roles, Optional<ulong[]> blacklist)
        {
            var callPerms = GetPerms(context.Guild?.Id, calls);

            var toUpdate = new List<CallPermission>();

            foreach (var call in callPerms)
            {
                var perm = call.Value ??
                    new CallPermission
                    {
                        CallName = call.Key.PermissionKey,
                        GuildId = context.Guild.Id,
                    };
                toUpdate.Add(perm);
                if (permId.IsSpecified)
                    perm.Permission = permId.Value;
                if (roles.IsSpecified)
                    perm.Roles = roles.Value;
                if (blacklist.IsSpecified)
                    perm.Blacklisted = blacklist.Value;
            }

            if (toUpdate.Count == 0)
                return;
            await Database.Upsert(toUpdate as IEnumerable<CallPermission>);
            CachedPermissions.Invalidate(context.Guild?.Id);
        }

        public async void ResetPermissions(IMessageContext context, CallInfo[] calls)
        {
            var callPerms = GetPerms(context.Guild?.Id, calls);

            var removing = callPerms.Values.Where(p => p != null);
            if (removing.Count() == 0)
                return;

            await Database.Delete(removing);
            CachedPermissions.Invalidate(context.Guild?.Id);
        }

        private Dictionary<CallInfo, CallPermission> GetPerms(ulong? guildId, params CallInfo[] calls)
        {
            var dict = CachedPermissions[guildId];
            if (dict.Count == 0)
                return calls.ToDictionary(c => c, c => (CallPermission)null);

            return calls.Distinct().ToDictionary(c => c, c => dict.TryGetValue(c.PermissionKey, out var perm) ? perm : null);
        }

    }
}
