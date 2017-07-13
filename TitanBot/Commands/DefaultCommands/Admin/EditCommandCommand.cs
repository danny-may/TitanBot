using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TitanBot.Commands.DefaultCommands.Admin
{
    [Description("EDITCOMMAND_HELP_DESCRIPTION")]
    [Notes("EDITCOMMAND_HELP_NOTES")]
    [DefaultPermission(8)]
    [RequireContext(ContextType.Guild)]
    public class EditCommandCommand : Command
    {
        IPermissionManager PermissionManager { get; }
        ICommandContext Context { get; }

        public EditCommandCommand(IPermissionManager permissions, ICommandContext context)
        {
            PermissionManager = permissions;
            Context = context;
        }

        IEnumerable<CallInfo> FindCalls(string[] cmds)
            =>  CommandService.CommandList.SelectMany(c => c.Calls)
                                       .Select(c => (Call: c, Path: c.PermissionKey.Split('.')))
                                       .Where(c => cmds.Count(t => c.Path.Zip(t.Split('.'), (p, v) => p.ToLower() == v.ToLower()).All(a => a)) > 0)
                                       .Select(c => c.Call);

        [Call("SetRole")]
        [Usage("EDITCOMMAND_HELP_USAGE_SETROLE")]
        async Task SetRoleAsync(string[] cmds, SocketRole[] roles = null)
        {
            var validCalls = FindCalls(cmds);
            
            if (validCalls == null || validCalls.Count() == 0)
            {
                await ReplyAsync(TextResource.GetResource("EDITCOMMAND_FINDCALLS_NORESULTS", ReplyType.Error));
                return;
            }

            PermissionManager.SetPermissions(Context, validCalls.ToArray(), null, roles.Select(r => r.Id).ToArray(), null);
            
            await ReplyAsync(TextResource.Format("EDITCOMMAND_SUCCESS", ReplyType.Success, "Roles", validCalls.Select(c => c.Parent).Distinct().Count()));
        }

        [Call("SetPerm")]
        [Usage("EDITCOMMAND_HELP_USAGE_SETPERM")]
        async Task SetPermAsync(string[] cmds, ulong permission)
        {
            var validCalls = FindCalls(cmds);

            if (validCalls == null || validCalls.Count() == 0)
            {
                await ReplyAsync(TextResource.GetResource("EDITCOMMAND_FINDCALLS_NORESULTS", ReplyType.Error));
                return;
            }

            PermissionManager.SetPermissions(Context, validCalls.ToArray(), permission, null, null);

            await ReplyAsync(TextResource.Format("EDITCOMMAND_SUCCESS", ReplyType.Success, "Permissions", validCalls.Select(c => c.Parent).Distinct().Count()));
        }

        [Call("Reset")]
        [Usage("EDITCOMMAND_HELP_USAGE_RESET")]
        async Task ResetCommandAsync(string[] cmds)
        {
            var validCalls = FindCalls(cmds);

            if (validCalls == null || validCalls.Count() == 0)
            {
                await ReplyAsync(TextResource.GetResource("EDITCOMMAND_FINDCALLS_NORESULTS", ReplyType.Error));
                return;
            }

            PermissionManager.ResetPermissions(Context, validCalls.ToArray());

            await ReplyAsync(TextResource.Format("EDITCOMMAND_SUCCESS", ReplyType.Success, "Permissions", validCalls.Select(c => c.Parent).Distinct().Count()));
        }

        [Call("Blacklist")]
        [Usage("EDITCOMMAND_HELP_USAGE_BLACKLIST")]
        async Task BlackListCommandAsync(string[] cmds, IMessageChannel[] channels)
        {
            var validCalls = FindCalls(cmds);

            if (validCalls == null || validCalls.Count() == 0)
            {
                await ReplyAsync(TextResource.GetResource("EDITCOMMAND_FINDCALLS_NORESULTS", ReplyType.Error));
                return;
            }

            PermissionManager.SetPermissions(Context, validCalls.ToArray(), null, null, channels.Select(c => c.Id).ToArray());

            await ReplyAsync(TextResource.Format("EDITCOMMAND_SUCCESS", ReplyType.Success, "Blacklist", validCalls.Select(c => c.Parent).Distinct().Count()));
        }
    }
}
