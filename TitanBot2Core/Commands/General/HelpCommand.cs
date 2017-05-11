using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.General
{
    public class HelpCommand : Command
    {
        public HelpCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => AllHelpAsync());
            Calls.AddNew(a => HelpAsync((string)a[0]))
                 .WithArgTypes(typeof(string));
            Description = "Displays help for any command";
            Usage.Add("`{0}` - Displays a list of all commands");
            Usage.Add("`{0} <command>` - Displays help for a specific command");
        }

        protected async Task AllHelpAsync()
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

            var commands = new List<CommandInfo>();
            foreach (var cmd in Context.CommandService.Commands)
            {
                var obj = cmd.CreateInstance(Context, Readers);
                var result = await obj.CheckCommandAsync();
                if (result.IsSuccess)
                    commands.Add(cmd);
            }
            var groups = commands.GroupBy(c => c.Group);
            foreach (var group in groups)
            {
                builder.AddField(group.Key, string.Join(", ", group.Select(g => g.Name)));
            }

            await ReplyAsync("", embed: builder);
        }

        private async Task HelpAsync(string name)
        {
            var searching = name.ToLower();
            var command = Context.CommandService.Commands.SingleOrDefault(c => c.Alias.Select(a => a.ToLower()).Contains(searching));

            if (command == null)
            {
                await ReplyAsync($"`{name}` is not a recognised command. Use `{Context.Prefix}help` for a list of all available commands", ReplyType.Error);
                return;
            }

            var usage = string.Join("\n", command.Usage.Select(u => string.Format(u, Context.Prefix + searching)));
            if (string.IsNullOrWhiteSpace(usage))
                usage = "No usage available!";
            else
                usage += "\n\n _`<param>` = required\n`[param]` = optional\n`<pram...>` = accepts multiple (comma separated)_";

            var aliases = string.Join(", ", command.Alias);

            var group = command.Group ?? "No categories!";

            var builder = new EmbedBuilder
            {
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Title = $"{Res.Str.InfoText} Help for {command.Name}",
                Description = command.Description ?? "No description",
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} | Help"
                }
            };

            builder.AddField("Group", group);
            if (!string.IsNullOrWhiteSpace(aliases))
                builder.AddField("Aliases", aliases);
            builder.AddField("Usage", usage);

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
