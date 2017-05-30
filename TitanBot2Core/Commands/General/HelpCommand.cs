using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.General
{
    [Description("Displays help for any command")]
    class HelpCommand : Command
    {
        [Call]
        [Usage("Displays a list of all commands, or help for a single command")]
        [CallFlag(typeof(string), "c", "Command to show help for")]
        async Task HelpAsync(string command = null)
        {
            if (command == null)
                await AllHelpAsync();
            else
                await HelpCommandAsync(command);
        }

        async Task AllHelpAsync()
        {
            var prefixes = await Context.GetPrefixes();
            var builder = new EmbedBuilder()
            {
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Title = $"{Res.Str.InfoText} These are the commands you can use",
                Description = $"To use a command, type `<prefix><command>`\n  e.g. `{prefixes.First()}help`\n"+
                              $"To pass arguments, add a list of values after the command separated by space\n  e.g. `{prefixes.First()}artifacts bos 200 500`\n" +
                              $"You can use any of these prefixes: \"{string.Join("\", \"", prefixes)}\"",
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} | Help"
                }
            };

            var commands = await Context.CommandService.FindAllowed(Context);

            var groups = commands.GroupBy(c => c.Key.Group);
            foreach (var group in groups)
            {
                builder.AddField(group.Key, string.Join(", ", group.GroupBy(g => g.Key.Name).Select(g => g.Key)));
            }

            await ReplyAsync("", embed: builder);
        }

        async Task HelpCommandAsync(string name)
        {
            name = name.ToLower();

            var classes = await Context.CommandService.FindAllowed(Context, name);

            if (classes.Count() != 1)
            { 
                await ReplyAsync($"`{name}` is not a recognised command. Use `{Context.Prefix}help` for a list of all available commands", ReplyType.Error);
                return;
            }

            var callInfos = classes.First();

            var usage = string.Join("\n", callInfos.Select(c => $"`{Context.Prefix + name + (" " + string.Join(" ", c.Subcalls.Concat(c.GetParameters()))).TrimEnd()}` - {c.Usage}"));
            if (string.IsNullOrWhiteSpace(usage))
                usage = "No usage available!";
            else
                usage += "\n\n_`<param>` = required\n`[param]` = optional\n`<param...>` = accepts multiple (comma separated)_";

            var aliases = callInfos.Key.Alias.Length == 0 ? "" : string.Join(", ", callInfos.Key.Alias.ToList());

            var group = callInfos.Key.Group ?? "No categories!";

            var flags = string.Join("\n", callInfos.SelectMany(c => c.Flags).GroupBy(f => f.ShortKey.ToLower()).Select(g => g.First()));

            var notes = callInfos.Key.Note;

            var builder = new EmbedBuilder
            {
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Title = $"{Res.Str.InfoText} Help for {callInfos.Key.Name}",
                Description = callInfos.Key.Description ?? "No description",
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} | Help"
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
