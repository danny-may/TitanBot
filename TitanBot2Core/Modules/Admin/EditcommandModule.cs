using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Preconditions;
using TitanBot2.Common;

namespace TitanBot2.Modules.Admin
{
    public partial class AdminModule
    {
        [Group("Editcommand")]
        [RequireContext(ContextType.Guild)]
        [RequireCustomPermission(8)]
        [Summary("Edits the role or permission required to use a given command")]
        public class EditcommandModule : TitanBotModule
        {
            private CommandService _service;

            public EditcommandModule(CommandService service)
            {
                _service = service;
            }

            private async Task<List<CommandInfo>> GetCommands(string cmds)
            {
                var commands = cmds.Split(',');

                var invalidCommands = new List<string>();
                var validCommands = new List<CommandInfo>();

                foreach (var command in commands)
                {
                    var results = _service.Commands.Where(c => c.Aliases.Select(a => a.Split(' ').FirstOrDefault() ?? "")
                                                                        .Any(a => a == command.ToLower()) &&
                                                                                  c.CheckPreconditionsAsync(Context).GetAwaiter().GetResult().IsSuccess);

                    if (results.Count() == 0)
                        invalidCommands.Add(command);
                    else
                        validCommands = validCommands.Concat(results).ToList();
                }

                if (invalidCommands.Count > 0)
                {
                    await ReplyAsync($"{Res.Str.ErrorText} The command(s) `{string.Join("`, `", invalidCommands)}` could not be found");
                    return null;
                }

                return validCommands;
            }

            [Command(RunMode = RunMode.Async)]
            [Remarks("FALSE")]
            public async Task DefaultCommand()
            {
                await ReplyAsync($"{Res.Str.ErrorText} You havent supplied enough arguments. Please use `{Context.Prefix}help <command>` for usage info");
            }

            [Command("SetRole", RunMode = RunMode.Async)]
            [Remarks("Sets the roles that are allowed to use the comma separated commands")]
            public async Task SetRoleAsync(string cmds, params SocketRole[] roles)
            {

                var validCommands = await GetCommands(cmds);

                if (validCommands == null)
                    return;

                foreach (var command in validCommands)
                {
                    if (roles.Length == 0)
                        await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, command.Name, null, null);
                    else
                        await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, command.Name, roles.Select(r => r.Id).ToArray(), null);
                }

                await ReplyAsync($"{Res.Str.SuccessText} Roles set successfully!");
            }

            [Command("SetPerm", RunMode = RunMode.Async)]
            [Remarks("Sets the permission required to use the comma separated commands")]
            public async Task SetPermAsync(string cmds, ulong permission)
            {
                var validCommands = await GetCommands(cmds);

                if (validCommands == null)
                    return;

                foreach (var command in validCommands)
                {
                    await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, command.Name, null, permission);
                }

                await ReplyAsync($"{Res.Str.SuccessText} Permissions set successfully!");
            }

            [Command("ResetCommand", RunMode = RunMode.Async)]
            [Remarks("Resets the role/permission requirements for the given command")]
            public async Task ResetCommandAsync(string cmds)
            {
                var validCommands = await GetCommands(cmds);

                if (validCommands == null)
                    return;

                foreach (var command in validCommands)
                {
                    await Context.Database.CmdPerms.SetCmdPerm(Context.Guild.Id, command.Name, null, null);
                }

                await ReplyAsync($"{Res.Str.SuccessText} Permissions reset successfully!");
            }
        }
    }
}
