using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Models;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.Data
{
    [Description("Displays data about any hero")]
    [RequireOwner]
    [Alias("Hero")]
    class HelpersCommand : Command
    {
        public HelpersCommand()
        {
            DelayMessage = "This might take a short while, theres a fair bit of data to download!";
        }

        [Call("List")]
        [Usage("Lists all heros available")]
        [CallFlag("g", "group", "Groups the heroes by damage")]
        async Task ListHelpersAsync()
        {
            var helpers = await Context.TT2DataService.GetAllHelpers();

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Name = "Hero listing"
                },
                Color = System.Drawing.Color.LightBlue.ToDiscord(),
                Description = "All Heros",
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} Hero tool"
                },
                Timestamp = DateTime.Now
            };

            if (!Flags.Has("g"))
                builder.AddField("Current Heroes", string.Join("\n", helpers.Where(h => h.IsInGame)
                                                                            .OrderBy(h => h.Order)
                                                                            .Select(h => $"{h.Name} - {h.HelperType.ToString().First()}")));
            else
                foreach (var group in helpers.Where(h => h.IsInGame)
                                             .OrderBy(h => h.Order)
                                             .GroupBy(h => h.HelperType))
                {
                    builder.AddField($"Current {group.Key} Heroes", "```\n" + group.Select(h => new string[] { h.Name, h.BaseCost.Beautify() + " gold"})
                                                                         .ToArray()
                                                                         .Tableify()+"\n```");
                }

            await ReplyAsync("", embed: builder.Build());
        }

        EmbedBuilder GetBaseEmbed(Helper helper)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "Hero data for " + helper.Name,
                    IconUrl = helper.ImageUrl,
                },
                ThumbnailUrl = helper.ImageUrl,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Text = $"{Context.Client.CurrentUser.Username} Hero tool | TT2 v{helper.FileVersion}"
                },
                Timestamp = DateTime.Now,
                Color = helper.Image.AverageColor(0.3f, 0.5f).ToDiscord(),
            };

            builder.AddInlineField("Hero id", helper.Id);
            builder.AddInlineField("Damage type", helper.HelperType);

            return builder;
        }

        [Call]
        [Usage("Shows stats for a given hero on the given level")]
        async Task ShowHelperAsync([Dense]Helper helper, int? level = null)
        {
            var builder = GetBaseEmbed(helper);

            if (level == null)
            {
                builder.AddInlineField("Base cost", helper.GetCost(0).Beautify());
                builder.AddInlineField("Base damage", helper.GetDps(1).Beautify());
            }
            else
            {
                builder.AddInlineField("Cost", helper.GetCost(0, level ?? 1).Beautify());
                builder.AddInlineField("Damage", helper.GetDps(level ?? 1).Beautify());
            }

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
