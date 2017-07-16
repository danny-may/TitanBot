using Discord;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Commands.Responses;
using TitanBot.Settings;
using TitanBot.Storage;
using TitanBot.Util;
using System;
using TitanBot.Commands.Models;
using System.Threading.Tasks;

namespace TitanBot.Commands
{
    class PermissionManager : IPermissionManager
    {
        private IDatabase Database { get; }
        private BotClient BotClient { get; }
        private ISettingsManager Settings { get; }
        private List<CallPermission> CachedPermissions { get; set; }

        public PermissionManager(IDatabase database, BotClient botClient, ISettingsManager settings)
        {
            Database = database;
            BotClient = botClient;
            Settings = settings;
            Task.Run(() => RefreshCache()).DontWait();
        }

        private async void RefreshCache()
        {
            while (true)
            {
                CachedPermissions = Database.Find<CallPermission>(x => true).Result.ToList();
                await Task.Delay(10000);
            }
        }

        public PermissionCheckResponse CheckAllowed(ICommandContext context, CallInfo[] calls)
        {
            var permitted = CheckContext(context, calls);
            if (permitted.Count() == 0)
                return PermissionCheckResponse.FromError("PERMISSIONMANAGER_DISALLOWED_NOTHERE");

            if (BotClient.Owners.Contains(context.Author.Id))
                return PermissionCheckResponse.FromSuccess(permitted);

            permitted = CheckOwner(context, permitted);
            if (permitted.Count() == 0)
                return PermissionCheckResponse.FromError("PERMISSIONMANAGER_DISALLOWED_NOTOWNER");

            if (context.Channel is IDMChannel || context.Channel is IGroupChannel || context.Guild.OwnerId == context.Author.Id)
                return PermissionCheckResponse.FromSuccess(permitted);

            var settings = Settings.GetGuildGroup<GeneralGuildSetting>(context.Guild.Id);
            if (DoesOverride(context, settings))
                return PermissionCheckResponse.FromSuccess(calls);

            permitted = CheckBlacklist(context, permitted);
            if (permitted.Count() == 0)
                return PermissionCheckResponse.FromError();

            permitted = CheckPermissions(context, permitted);
            if (permitted.Count() == 0)
                return PermissionCheckResponse.FromError("PERMISSIONMANAGER_DISALLOWED_NOPERMISSION");

            return PermissionCheckResponse.FromSuccess(permitted);

        }

        private bool DoesOverride(ICommandContext context, GeneralGuildSetting settings)
        {
            if (settings.RoleOverride != null && ((context.Author as IGuildUser)?.RoleIds.Any(r => settings.RoleOverride.Contains(r)) ?? false))
                return true;
            if ((context.Author as IGuildUser)?.HasAll(settings.PermOverride) ?? false)
                return true;
            return false;
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

                return isValid && (c.Parent.RequireGuild == null || c.Parent.RequireGuild == context.Guild?.Id);
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

        public CallInfo[] CheckBlacklist(ICommandContext context, CallInfo[] calls)
        {
            var guildUser = context.Author as IGuildUser;

            if (guildUser.HasAll(context.GuildData.PermOverride))
                return calls;

            if ((context.GuildData.BlackListed?.Length ?? 0) != 0)
                if (context.GuildData.BlackListed.Contains(context.Channel.Id))
                    return new CallInfo[0];

            return calls.Where(c => {
                var permission = CachedPermissions.FirstOrDefault(p => p.CallName.ToLower() == c.PermissionKey.ToLower());
                if (permission == null)
                    return true;
                if (permission.Blacklisted != null)
                    return !permission.Blacklisted.Contains(context.Channel.Id);
                return true;
            }).ToArray();
        }

        public CallInfo[] CheckPermissions(ICommandContext context, CallInfo[] calls)
        {
            var guildUser = context.Author as IGuildUser;

            return calls.Where(c =>
            {
                var permission = CachedPermissions.FirstOrDefault(p => p.CallName.ToLower() == c.PermissionKey.ToLower());
                if (permission == null || (permission.Roles == null || permission.Roles.Count() == 0))
                    return guildUser.HasAll(permission?.Permission ?? c.DefaultPermissions);
                return permission.Roles != null && guildUser.RoleIds.Any(r => permission.Roles.Contains(r));
            }).ToArray();
        }

        public async void SetPermissions(ICommandContext context, CallInfo[] calls, ulong? permission, ulong[] roles, ulong[] blacklist)
        {
            foreach (var call in calls)
            {
                var current = CachedPermissions.FirstOrDefault(p => p.CallName.ToLower() == call.PermissionKey.ToLower() && p.GuildId == context.Guild.Id);
                if (current == null)
                {
                    current = new CallPermission
                    {
                        CallName = call.PermissionKey,
                        GuildId = context.Guild.Id,
                    };
                    CachedPermissions.Add(current);
                }
                if (permission != null)
                    current.Permission = permission;
                if (roles != null)
                    current.Roles = roles;
                if (blacklist != null)
                    current.Blacklisted = blacklist;
                await Database.Upsert(current);
            }
        }

        public async void ResetPermissions(ICommandContext context, CallInfo[] calls)
        {
            foreach (var call in calls)
            {
                var current = CachedPermissions.FirstOrDefault(p => p.CallName.ToLower() == call.PermissionKey.ToLower() && p.GuildId == context.Guild.Id);
                if (current == null)
                    continue;
                CachedPermissions.Remove(current);
                await Database.Delete(current);
            }
        }
    }
}
