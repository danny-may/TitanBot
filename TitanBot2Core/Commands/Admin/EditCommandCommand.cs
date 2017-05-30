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
        IEnumerable<CommandInfo> FindCommands(string[] cmds)
            => Context.CommandService.Commands.Where(c => c.Alias.Any(a => cmds.Select(s => s.ToLower()).Contains(a.ToLower()))); 

        [Call("SetRole")]
        [Usage("Sets a list of roles required to use each command supplied")]
        async Task SetRoleAsync(string[] cmds, SocketRole[] roles)
        {
            var validCommands = FindCommands(cmds);
            
            if (validCommands == null)
                return;
            
            foreach (var command in validCommands)
            {
                if (roles.Length == 0)
                    await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, command.Name, null, null);
                else
                    await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, command.Name, roles.Select(r => r.Id).ToArray(), null);
            }
            
            await ReplyAsync("Roles set successfully!", ReplyType.Success);
        }

        [Call("SetPerm")]
        [Usage("Sets a permission required to use each command supplied")]
        async Task SetPermAsync(string[] cmds, ulong permission)
        {
            var validCommands = FindCommands(cmds);

            if (validCommands == null)
                return;

            foreach (var command in validCommands)
            {
                await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, command.Name, null, permission);
            }

            await ReplyAsync("Permissions set successfully!", ReplyType.Success);
        }

        [Call("Reset")]
        [Usage("Resets the roles and permissions required to use each command supplied")]
        async Task ResetCommandAsync(string[] cmds)
        {
            var validCommands = FindCommands(cmds);

            if (validCommands == null)
                return;

            foreach (var command in validCommands)
            {
                await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, command.Name, null, null);
            }

            await ReplyAsync("Permissions reset successfully!", ReplyType.Success);
        }

        [Call("Blacklist")]
        [Usage("Prevents anyone with permissions below the override permissions from using the command in the given channel")]
        async Task BlackListCommandAsync(string[] cmds, IMessageChannel[] channels)
        {
            var validCommands = FindCommands(cmds);

            if (validCommands == null)
                return;

            foreach (var command in validCommands)
            {
                await Context.Database.CmdPerms.BlackList(Context.Guild.Id, command.Name, channels.Select(c => c.Id).ToArray(), true);
            }

            await ReplyAsync($"Blacklisted {validCommands.Count()} command(s) from {channels.Length} channel(s)!", ReplyType.Success);
        }

        [Call("Whitelist")]
        [Usage("Reenables the use of the commands in the channels")]
        async Task WhiteListCommandAsync(string[] cmds, IMessageChannel[] channels)
        {
            var validCommands = FindCommands(cmds);

            if (validCommands == null)
                return;

            foreach (var command in validCommands)
            {
                await Context.Database.CmdPerms.BlackList(Context.Guild.Id, command.Name, channels.Select(c => c.Id).ToArray(), false);
            }

            await ReplyAsync($"Whitelisted {validCommands.Count()} command(s) from {channels.Length} channel(s)!", ReplyType.Success);
        }
    }
}
