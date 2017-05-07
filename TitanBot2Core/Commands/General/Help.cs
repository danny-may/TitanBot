using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.General
{
    public class Help : Command
    {
        public Help(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            SubCommands.Add(".*", HelpAsync);
            Description = "Displays help for any command";
            Usage = new string[]
            {
                "`{0}` - Displays a list of all commands",
                "`{0} <command>` - Displays help for a specific command"
            };
        }

        protected override async Task RunAsync()
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

            var groups = Context.CommandService.Commands.GroupBy(c => c.Group);
            foreach (var group in groups)
            {
                builder.AddField(group.Key, string.Join(", ", group.Select(g => g.Name)));
            }

            await ReplyAsync("", embed: builder);
        }

        private async Task HelpAsync()
        {
            var searching = Context.Arguments.First().ToLower();
            var command = Context.CommandService.Commands.SingleOrDefault(c => c.Name.ToLower() == searching);

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

            builder.AddField("Usage", usage);
            if (!string.IsNullOrWhiteSpace(aliases))
                builder.AddField("Aliases", aliases);
            builder.AddField("Group", group);


            await ReplyAsync("", embed: builder.Build());
        }

        protected override async Task<CommandCheckResponse> CheckArguments()
        {
            if (Context.Arguments.Length == 0)
                return CommandCheckResponse.FromSuccess();
            if (Context.Arguments.Length == 1 && Context.CommandService.Commands.SingleOrDefault(c => c.Name.ToLower() == Context.Arguments.First().ToLower()) != null)
                return CommandCheckResponse.FromSuccess();
            return CommandCheckResponse.FromError("The command `" + Context.Arguments.First() + "` was not recognised.");
        }
    }
}
