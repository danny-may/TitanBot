using DC = Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Admin
{
    public class EditCommandCommand : Command
    {
        public EditCommandCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => SetRoleAsync((string[])a[0], (SocketRole[])a[1]))
                 .WithArgTypes(typeof(string[]), typeof(SocketRole[]))
                 .WithSubCommand("SetRole");
            Calls.AddNew(a => SetPermAsync((string[])a[0], (ulong)a[1]))
                 .WithArgTypes(typeof(string[]), typeof(ulong))
                 .WithSubCommand("SetPerm");
            Calls.AddNew(a => ResetCommandAsync((string[])a[0]))
                 .WithArgTypes(typeof(string[]))
                 .WithSubCommand("Reset");
            DefaultPermission = 8;
            Usage.Add("`{0} setrole <commands..> <roles..>` - Sets a list of roles required to use each command supplied");
            Usage.Add("`{0} setperm <commands..> <permission>` - Sets a permission required to use each command supplied");
            Usage.Add("`{0} reset <commands..>` - Resets the roles and permissions required to use each command supplied");
            Description = "Used to allow people with varying roles or permissions to use different commands.";
            RequiredContexts = DC.ContextType.Guild;
        }

        public IEnumerable<CommandInfo> FindCommands(string[] cmds)
            => Context.CommandService.Commands.Where(c => c.Alias.Any(a => cmds.Select(s => s.ToLower()).Contains(a.ToLower()))); 

        public async Task SetRoleAsync(string[] cmds, SocketRole[] roles)
        {
            var validCommands = FindCommands(cmds);
            
            if (validCommands == null)
                return;
            
            foreach (var command in validCommands)
            {
                if (roles.Length == 0)
                    await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, command.Name, null, null, null);
                else
                    await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, command.Name, null, roles.Select(r => r.Id).ToArray(), null);
            }
            
            await ReplyAsync("Roles set successfully!", ReplyType.Success);
        }

        public async Task SetPermAsync(string[] cmds, ulong permission)
        {
            var validCommands = FindCommands(cmds);

            if (validCommands == null)
                return;

            foreach (var command in validCommands)
            {
                await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, command.Name, null, null, permission);
            }

            await ReplyAsync("Permissions set successfully!", ReplyType.Success);
        }

        public async Task ResetCommandAsync(string[] cmds)
        {
            var validCommands = FindCommands(cmds);

            if (validCommands == null)
                return;

            foreach (var command in validCommands)
            {
                await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, command.Name, null, null, null);
            }

            await ReplyAsync("Permissions reset successfully!", ReplyType.Success);
        }
    }
}
