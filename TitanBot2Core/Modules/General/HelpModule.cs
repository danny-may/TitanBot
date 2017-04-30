using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Preconditions;

namespace TitanBot2.Modules.General
{
    public partial class GeneralModule
    {
        [Group("Help")]
        [Summary("Displays help for the bot commands")]
        [RequireCustomPermission(0)]
        public class HelpModule : TitanBotModule
        {
            private CommandService _service;

            public HelpModule(CommandService service)
            {
                _service = service;
            }

            [Command(RunMode = RunMode.Async)]
            [Remarks("Displays a list of all the commands available to you")]
            public async Task HelpAsync()
            {
                var prefixes = await Context.GetPrefixes();
                var builder = new EmbedBuilder()
                {
                    Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                    Title = $"{Res.Str.InfoText} These are the commands you can use",
                    Description = prefixes.Length > 0 ? $"You can use any of these prefixes: \"{string.Join("\", \"", prefixes)}\"" : null,
                    Timestamp = DateTime.Now,
                    Footer = new EmbedFooterBuilder
                    {
                        IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                        Text = $"{Context.Client.CurrentUser.Username} | Help"
                    }
                };

                foreach (var module in _service.Modules.Where(m => m.IsSubmodule == false)
                                                       .GroupBy(m => m.Name))
                {
                    var description = new List<string>();
                    foreach (var child in module.Where(m => (m.Submodules?.Count ?? 0) > 0)
                                                .SelectMany(m => m.Submodules))
                    {
                        var result = false;
                        foreach (var cmd in child.Commands)
                        {
                            if ((await cmd.CheckPreconditionsAsync(Context)).IsSuccess)
                            {
                                result = true;
                                break;
                            }
                        }
                        if (result)
                            description.Add(child.Aliases.First());
                    }

                    description = description.Distinct().ToList();

                    if (description.Count != 0)
                    {
                        builder.AddField(x =>
                        {
                            x.Name = module.Key;
                            x.Value = string.Join(", ", description);
                            x.IsInline = false;
                        });
                    }
                }

                await ReplyAsync("", false, builder.Build());
            }

            [Command(RunMode = RunMode.Async)]
            [Remarks("Displays help information for the given command")]
            public async Task HelpAsync(string command)
            {
                var cmds = _service.Commands.Where(c => c.Aliases.Select(a => a.Split(' ').FirstOrDefault() ?? "")
                                                                 .Any(a => a == command.ToLower()) &&
                                                        c.CheckPreconditionsAsync(Context).GetAwaiter().GetResult().IsSuccess);

                if (cmds.Count() == 0)
                {
                    await ReplyAsync($"{Res.Str.ErrorText} Could not find a command by that name!");
                    return;
                }

                var categories = string.Join(", ", cmds.Select(c => c.GetTopParent()).Distinct().Select(c => c.Name));
                if (string.IsNullOrWhiteSpace(categories))
                    categories = "No categories!";

                var usage = string.Join("\n", cmds.Where(c => c.Remarks != null)
                                                  .Select(c => $"```{Context.Prefix}{c.Aliases.First()} {string.Join(" ", c.Parameters.Select(p => p.ToHelpString()))}``` - {c.Remarks}"));
                if (string.IsNullOrWhiteSpace(usage))
                    usage = "No usage available!";

                var descriptions = cmds.Where(c => !string.IsNullOrWhiteSpace(c.FindSummary())).Select(c => c.FindSummary()).Distinct().ToList();
                string description;
                if (descriptions.Count < 2)
                    description = descriptions.FirstOrDefault();
                else
                    description = string.Join("\n", descriptions.Select(d => $"{descriptions.IndexOf(d) + 1}. {d}"));
                if (string.IsNullOrWhiteSpace(description))
                    description = "No description available!";

                var name = string.Join(", ", cmds.Select(c => c.GetTopParent(1)?.Name ?? c.Name).Distinct());

                var aliases = string.Join(", ", cmds.SelectMany(c => c.GetTopParent(1)?.Aliases ?? c.Aliases)
                                                    .Where(a => !string.IsNullOrWhiteSpace(a))
                                                    .Select(a => a.ToLower())
                                                    .Distinct());

                var builder = new EmbedBuilder
                {
                    Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                    Title = $"{Res.Str.InfoText} Help for {name}",
                    Description = description,
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
                builder.AddField("Categories", categories);

                await ReplyAsync("", embed: builder.Build());
            }
        }
    }
}
