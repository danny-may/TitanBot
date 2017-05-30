using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Commands.Admin
{
    [Description("Used to allow people with varying roles or permissions to use different commands.")]
    [Notes("To work out just what permission id you need, give the [permission calculator](https://discordapi.com/permissions.html) a try!")]
    [DefaultPermission(8)]
    [RequireContext(ContextType.Guild)]
    class EditCommandCommand : Command
    {
        IEnumerable<CallInfo> FindCalls(string[] cmds)
        {
            cmds = cmds.Select(c => c.ToLower()).ToArray();
            return Context.CommandService.Commands.SelectMany(c => c.Calls)
                                                  .Where(c => cmds.Any(s => c.Matches(s)));
        }

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
            
            foreach (var key in validCalls.Select(c => c.PermissionKey).Distinct())
            { 
                if (roles == null || roles.Length == 0)
                    await Context.Database.CmdPerms.ResetCmdPerm(Context.Guild.Id, key);
                else
                    await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, key, roles.Select(r => r.Id).ToArray(), null);
            }
            
            await ReplyAsync("Roles set successfully!", ReplyType.Success);
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

            foreach (var key in validCalls.Select(c => c.PermissionKey).Distinct())
            {
                if (permission == null)
                    await Context.Database.CmdPerms.ResetCmdPerm(Context.Guild.Id, key);
                else
                    await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, key, null, permission);
            }

            await ReplyAsync("Permissions set successfully!", ReplyType.Success);
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

            foreach (var key in validCalls.Select(c => c.PermissionKey).Distinct())
            {
                await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, key, null, null);
            }

            await ReplyAsync("Permissions reset successfully!", ReplyType.Success);
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

            foreach (var key in validCalls.Select(c => c.PermissionKey).Distinct())
            {
                await Context.Database.CmdPerms.BlackList(Context.Guild.Id, key, channels.Select(c => c.Id).ToArray(), true);
            }

            await ReplyAsync($"Blacklisted {validCalls.Select(c => c.ParentInfo).Distinct().Count()} command(s) from {channels.Length} channel(s)!", ReplyType.Success);
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

            foreach (var key in validCalls.Select(c => c.PermissionKey).Distinct())
            {
                await Context.Database.CmdPerms.BlackList(Context.Guild.Id, key, channels.Select(c => c.Id).ToArray(), false);
            }

            await ReplyAsync($"Whitelisted {validCalls.Select(c => c.ParentInfo).Distinct().Count()} command(s) from {channels.Length} channel(s)!", ReplyType.Success);
        }
    }
}
