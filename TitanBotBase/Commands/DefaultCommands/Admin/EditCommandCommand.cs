using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.Database;
using TitanBotBase.Database.Tables;

namespace TitanBotBase.Commands.DefaultCommands.Admin
{
    [Description("Used to allow people with varying roles or permissions to use different commands.")]
    [Notes("To work out just what permission id you need, give the [permission calculator](https://discordapi.com/permissions.html) a try!")]
    [DefaultPermission(8)]
    [RequireContext(ContextType.Guild)]
    class EditCommandCommand : Command
    {
        IEnumerable<CallInfo> FindCalls(string[] cmds)
            =>  CommandService.Commands.SelectMany(c => c.Calls)
                                       .Select(c => (Call: c, Path: c.PermissionKey.Split('.')))
                                       .Where(c => cmds.Count(t => c.Path.Zip(t.Split('.'), (p, v) => p.ToLower() == v.ToLower()).All(a => a)) > 0)
                                       .Select(c => c.Call);

        [Call("SetRole")]
        [Usage("Sets a list of roles required to use each command supplied")]
        async Task SetRoleAsync(string[] cmds, SocketRole[] roles = null)
        {
            var validCalls = FindCalls(cmds);
            
            if (validCalls == null || validCalls.Count() == 0)
            {
                await ReplyAsync("There were no commands that matched those calls.", ReplyType.Error);
                return;
            }

            var currentPerms = await Database.Find<CallPermission>(p => p.GuildId == Guild.Id);

            foreach (var key in validCalls.Select(c => c.PermissionKey).Distinct())
            {
                var current = currentPerms.FirstOrDefault(p => p.CallName.ToLower() == key.ToLower()) ?? new CallPermission
                {
                    CallName = key,
                    GuildId = Guild.Id,
                };
                current.Roles = roles?.Select(r => r.Id).ToArray();
                await Database.Upsert(current);
            }
            
            await ReplyAsync($"Roles set successfully for {validCalls.Select(c => c.Parent).Distinct().Count()} command(s)!", ReplyType.Success);
        }

        [Call("SetPerm")]
        [Usage("Sets a permission required to use each command supplied")]
        async Task SetPermAsync(string[] cmds, ulong? permission = null)
        {
            var validCalls = FindCalls(cmds);

            if (validCalls == null || validCalls.Count() == 0)
            {
                await ReplyAsync("There were no commands that matched those calls.", ReplyType.Error);
                return;
            }

            var currentPerms = await Database.Find<CallPermission>(p => p.GuildId == Guild.Id);

            foreach (var key in validCalls.Select(c => c.PermissionKey).Distinct())
            {
                var current = currentPerms.FirstOrDefault(p => p.CallName.ToLower() == key.ToLower()) ?? new CallPermission
                {
                    CallName = key,
                    GuildId = Guild.Id,
                };
                current.Permission = permission;
                await Database.Upsert(current);
            }

            await ReplyAsync($"Permissions set successfully for {validCalls.Select(c => c.Parent).Distinct().Count()} command(s)!", ReplyType.Success);
        }

        [Call("Reset")]
        [Usage("Resets the roles and permissions required to use each command supplied")]
        async Task ResetCommandAsync(string[] cmds)
        {
            var validCalls = FindCalls(cmds);

            if (validCalls == null || validCalls.Count() == 0)
            {
                await ReplyAsync("There were no commands that matched those calls.", ReplyType.Error);
                return;
            }

            var currentPerms = await Database.Find<CallPermission>(p => p.GuildId == Guild.Id);
            var resetting = currentPerms.Where(p => validCalls.Any(c => c.PermissionKey.ToLower() == p.CallName.ToLower()));

            await Database.Delete(resetting);

            await ReplyAsync($"Permissions reset successfully for {validCalls.Select(c => c.Parent).Distinct().Count()} command(s)!", ReplyType.Success);
        }

        [Call("Blacklist")]
        [Usage("Prevents anyone with permissions below the override permissions from using the command in the given channel")]
        async Task BlackListCommandAsync(string[] cmds, IMessageChannel[] channels)
        {
            var validCalls = FindCalls(cmds);

            if (validCalls == null || validCalls.Count() == 0)
            {
                await ReplyAsync("There were no commands that matched those calls.", ReplyType.Error);
                return;
            }

            var currentPerms = await Database.Find<CallPermission>(p => p.GuildId == Guild.Id);

            foreach (var key in validCalls.Select(c => c.PermissionKey).Distinct())
            {
                var current = currentPerms.FirstOrDefault(p => p.CallName.ToLower() == key.ToLower()) ?? new CallPermission
                {
                    CallName = key,
                    GuildId = Guild.Id,
                };
                var newBlackList = new List<ulong>(current.Blacklisted ?? new ulong[0] { });
                newBlackList.AddRange(channels.Select(c => c.Id));
                current.Blacklisted = newBlackList.ToArray();
                await Database.Upsert(current);
            }

            await ReplyAsync($"Blacklisted {validCalls.Select(c => c.Parent).Distinct().Count()} call(s) from {channels.Length} channel(s)!", ReplyType.Success);
        }

        [Call("Whitelist")]
        [Usage("Reenables the use of the commands in the channels")]
        async Task WhiteListCommandAsync(string[] cmds, IMessageChannel[] channels)
        {
            var validCalls = FindCalls(cmds);

            if (validCalls == null || validCalls.Count() == 0)
            {
                await ReplyAsync("There were no commands that matched those calls.", ReplyType.Error);
                return;
            }

            var currentPerms = await Database.Find<CallPermission>(p => p.GuildId == Guild.Id);

            foreach (var key in validCalls.Select(c => c.PermissionKey).Distinct())
            {
                var current = currentPerms.FirstOrDefault(p => p.CallName.ToLower() == key.ToLower()) ?? new CallPermission
                {
                    CallName = key,
                    GuildId = Guild.Id,
                };
                var newBlackList = new List<ulong>(current.Blacklisted ?? new ulong[0] { });
                newBlackList.RemoveAll(u => channels.Select(c => c.Id).Contains(u));
                current.Blacklisted = newBlackList.ToArray();
                await Database.Upsert(current);
            }

            await ReplyAsync($"Whitelisted {validCalls.Select(c => c.Parent).Distinct().Count()} command(s) in {channels.Length} channel(s)!", ReplyType.Success);
        }
    }
}
