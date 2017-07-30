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
                await AllHelpAsync();
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

        async Task AllHelpAsync()
        {
            var builder = new EmbedBuilder()
            {
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Title = TextResource.GetResource("HELP_LIST_TITLE", ReplyType.Info),
                Description = TextResource.Format("HELP_LIST_DESCRIPTION", Prefix, string.Join("\", \"", AcceptedPrefixes)),
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = TextResource.Format("EMBED_FOOTER", BotUser.Username, "Help")
                }
            };

            var commands = FindPermitted().Where(c => !c.Hidden);

            var groups = commands.GroupBy(c => c.Group);
            foreach (var group in groups)
            {
                builder.AddField(group.Key, string.Join(", ", group.GroupBy(g => g.Name).Select(g => g.Key)));
            }
            
            await ReplyAsync(Embedable.FromEmbed(builder));
        }

        async Task HelpCommandAsync(string name)
        {
            var cmd = CommandService.Search(name, out int commandLength);
            if (cmd == null || cmd.Value.Hidden)
            {
                await ReplyAsync("HELP_SINGLE_UNRECOGNISED", ReplyType.Error, name, Prefix);
                return;
            }
            var command = cmd.Value;

            name = name.Substring(0, commandLength).Trim();

            var permCheckResponse = PermissionManager.CheckAllowed(Context, command.Calls.ToArray());
            if (!permCheckResponse.IsSuccess)
            {
                if (permCheckResponse.ErrorMessage != null)
                    await ReplyAsync(permCheckResponse.ErrorMessage, ReplyType.Error);
                return;
            }

            var permitted = permCheckResponse.Permitted.Where(c => !c.Hidden);

            var usages = new List<string>();
            foreach (var call in permitted)
            {
                var pfx = Context.Prefix;
                var nme = name;
                var parm = " " + call.SubCall + " " + string.Join(" ", call.GetParameters());
                var flgs = " " + call.GetFlags();
                usages.Add($"`{pfx + nme + parm.TrimEnd() + flgs.TrimEnd()}` - {TextResource.GetResource(call.Usage)}");
            }

            var notes = TextResource.GetResource(command.Note);

            var usage = string.Join("\n", usages);
            if (string.IsNullOrWhiteSpace(usage))
                usage = TextResource.GetResource("HELP_SINGLE_NOUSAGE");
            else
                notes += TextResource.GetResource("HELP_SINGLE_USAGE_FOOTER");
            
            var aliases = command.Alias.Length == 0 ? "" : string.Join(", ", command.Alias.ToList());
            
            var group = command.Group ?? TextResource.GetResource("HELP_SINGLE_NOGROUP");

            var flags = string.Join("\n", permitted.SelectMany(c => c.Flags)
                                                   .GroupBy(f => f.ShortKey)
                                                   .Select(g => g.First()));
            
            var builder = new EmbedBuilder
            {
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Title = TextResource.Format("HELP_SINGLE_TITLE", ReplyType.Info, command.Name),
                Description = TextResource.GetResource(command.Description ?? "HELP_SINGLE_NODESCRIPTION"),
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = TextResource.Format("EMBED_FOOTER", BotUser.Username, "Help")
                }
            };
            
            builder.AddInlineField(TextResource.GetResource("GROUP"), group);
            if (!string.IsNullOrWhiteSpace(aliases))
                builder.AddInlineField(TextResource.GetResource("ALIASES"), Format.Sanitize(aliases));
            builder.AddField(TextResource.GetResource("USAGE"), usage);
            if (!string.IsNullOrWhiteSpace(flags))
                builder.AddField(TextResource.GetResource("FLAGS"), flags);
            if (!string.IsNullOrWhiteSpace(notes))
                builder.AddField(TextResource.GetResource("NOTES"), notes);
            
            await ReplyAsync(Embedable.FromEmbed(builder));
        }
    }
}
