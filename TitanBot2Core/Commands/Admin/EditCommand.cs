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
                 .WithSubCommand("ResetCommand");
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
            
            await ReplyAsync($"{Res.Str.SuccessText} Roles set successfully!");
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

            await ReplyAsync($"{Res.Str.SuccessText} Permissions set successfully!");
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

            await ReplyAsync($"{Res.Str.SuccessText} Permissions reset successfully!");
        }
    }
}
