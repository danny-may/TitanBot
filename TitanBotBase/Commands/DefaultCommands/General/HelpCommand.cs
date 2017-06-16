using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.Commands.Responses;
using TitanBotBase.Util;

namespace TitanBotBase.Commands.DefautlCommands.General
{
    [Description("Displays help for any command")]
    class HelpCommand : Command
    {
        private IPermissionChecker PermChecker { get; }
        private ICommandContext Context { get; }

        public HelpCommand(IPermissionChecker checker, ICommandContext context)
        {
            PermChecker = checker;
            Context = context;
        }

        [Call]
        [Usage("Displays a list of all commands, or help for a single command")]
        async Task HelpAsync(string command = null)
        {
            if (command == null)
                await AllHelpAsync();
            else
                await HelpCommandAsync(command);
        }


        CommandInfo[] FindPermitted()
            => CommandService.CommandList.Where(c => FindPermitted(c).IsSuccess).ToArray();

        PermissionCheckResponse FindPermitted(CommandInfo command)
            => PermChecker.CheckAllowed(Context, command.Calls.ToArray());

        async Task AllHelpAsync()
        {
            var builder = new EmbedBuilder()
            {
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Title = $":information_source: These are the commands you can use",
                Description = $"To use a command, type `<prefix><command>`\n  e.g. `{Prefix}help`\n"+
                              $"To pass arguments, add a list of values after the command separated by space\n  e.g. `{Prefix}artifacts bos 200 500`\n" +
                              $"You can use any of these prefixes: \"{string.Join("\", \"", AcceptedPrefixes)}\"",
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = $"{BotUser.Username} | Help"
                }
            };

            var commands = FindPermitted();

            var groups = commands.GroupBy(c => c.Group);
            foreach (var group in groups)
            {
                builder.AddField(group.Key, string.Join(", ", group.GroupBy(g => g.Name).Select(g => g.Key)));
            }
            
            await ReplyAsync("", embed: builder);
        }

        async Task HelpCommandAsync(string name)
        {
            var cmd = CommandService.Search(name, out int commandLength);
            if (cmd == null)
            {
                await ReplyAsync($"`{name}` is not a recognised command. Use `{Context.Prefix}help` for a list of all available commands", ReplyType.Error);
                return;
            }
            var command = cmd.Value;

            name = name.Substring(0, commandLength).Trim();

            var permCheckResponse = PermChecker.CheckAllowed(Context, command.Calls.ToArray());
            if (!permCheckResponse.IsSuccess)
            {
                if (permCheckResponse.ErrorMessage != null)
                    await ReplyAsync(permCheckResponse.ErrorMessage, ReplyType.Error);
                return;
            }

            var permitted = permCheckResponse.Permitted;

            var usages = new List<string>();
            foreach (var call in permitted)
            {
                var pfx = Context.Prefix;
                var nme = name;
                var parm = " " + call.SubCall + " " + string.Join(" ", call.GetParameters());
                var flgs = " " + call.GetFlags();
                usages.Add($"`{pfx + nme + parm.TrimEnd() + flgs.TrimEnd()}` - {call.Usage}");
            }
            var usage = string.Join("\n", usages);
            if (string.IsNullOrWhiteSpace(usage))
                usage = "No usage available!";
            else
                usage += "\n\n_`<param>` = required\n`[param]` = optional\n`<param...>` = accepts multiple (comma separated)_";
            
            var aliases = command.Alias.Length == 0 ? "" : string.Join(", ", command.Alias.ToList());
            
            var group = command.Group ?? "No categories!";
            
            var flags = string.Join("\n", permitted.SelectMany(c => c.Flags)
                                                   .GroupBy(f => f.ShortKey)
                                                   .Select(g => g.First()));
            
            var notes = command.Note;
            
            var builder = new EmbedBuilder
            {
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Title = DiscordUtil.FormatMessage($"Help for {command.Name}", 2),
                Description = command.Description ?? "No description",
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = $"{BotUser.Username} | Help"
                }
            };
            
            builder.AddInlineField("Group", group);
            if (!string.IsNullOrWhiteSpace(aliases))
                builder.AddInlineField("Aliases", Format.Sanitize(aliases));
            builder.AddField("Usage", usage);
            if (!string.IsNullOrWhiteSpace(flags))
                builder.AddField("Flags", flags);
            if (!string.IsNullOrWhiteSpace(notes))
                builder.AddField("Notes", notes);
            
            await ReplyAsync("", embed: builder.Build());
        }
    }
}
