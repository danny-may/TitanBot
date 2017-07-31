using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Commands.Responses;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description("HELP_HELP_DESCRIPTION")]
    public class HelpCommand : Command
    {
        private IPermissionManager PermissionManager { get; }
        public Dictionary<string, TutorialModel> Tutorials { get; } = new Dictionary<string, TutorialModel>();

        public HelpCommand(IPermissionManager checker)
        {
            PermissionManager = checker;
        }

        [Call]
        [Usage("HELP_HELP_USAGE")]
        async Task HelpAsync(string command = null)
        {
            if (command == null)
                await ReplyAsync(new HelpGeneralEmbedable(FindPermitted().Where(c => !c.Hidden), Context, AcceptedPrefixes));
            else
                await HelpCommandAsync(command);
        }

        [Call("Tutorial")]
        [Usage("HELP_HELP_USAGE_TUTORIAL")]
        async Task TutorialAsync(string tutorialArea)
        {
            await Task.Delay(0);
        }


        CommandInfo[] FindPermitted()
            => CommandService.CommandList.Where(c => FindPermitted(c).IsSuccess).ToArray();

        PermissionCheckResponse FindPermitted(CommandInfo command)
            => PermissionManager.CheckAllowed(Context, command.Calls.ToArray());

        async Task HelpCommandAsync(string name)
        {
            var cmd = CommandService.Search(name, out int commandLength);
            if (cmd == null || cmd.Value.Hidden)
            {
                await ReplyAsync("HELP_SINGLE_UNRECOGNISED", ReplyType.Error, name, Prefix);
                return;
            }

            name = name.Substring(0, commandLength).Trim();

            var permCheckResponse = PermissionManager.CheckAllowed(Context, cmd.Value.Calls.ToArray());
            if (!permCheckResponse.IsSuccess)
            {
                if (permCheckResponse.ErrorMessage != null)
                    await ReplyAsync(permCheckResponse.ErrorMessage, ReplyType.Error);
                return;
            }

            var permitted = permCheckResponse.Permitted.Where(c => !c.Hidden);
            
            await ReplyAsync(new HelpCommandEmbeddable(cmd.Value, permitted, name, Context));
        }
    }
}
